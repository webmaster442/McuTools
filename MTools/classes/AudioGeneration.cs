using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MTools.classes
{
    public enum WaveType
    {
        Sinus, Square, Saw, Noise, None
    }

    public interface IOscillator
    {
        WaveType Wavetype { get; }
        double OscFrequency { get; }
        double OscAmplitude { get; }
        bool Running { get; set; }
    }

    public class Mixer : IDisposable
    {
        private List<IOscillator> _oscillators;
        private const int MAX_AMPLITUDE = 32760; // for 16-bit audio\
        private WaveHeader header;
        private WaveFormatChunk format;
        private WaveDataChunk data;
        private string _targetfile;
        private uint _numSamples;
        private SoundPlayer _sndp;

        public Mixer()
        {
            _oscillators = new List<IOscillator>();
            header = new WaveHeader();
            format = new WaveFormatChunk();
            data = new WaveDataChunk();
            _targetfile = System.IO.Path.GetTempFileName();
            _numSamples = format.dwSamplesPerSec * 3 * format.wChannels;
            data.shortArray = new short[_numSamples];
            _sndp = new SoundPlayer();
        }

        ~Mixer()
        {
            Dispose(true);
        }

        public List<IOscillator> Oscillator
        {
            get { return _oscillators; }
        }

        public Canvas DrawArea { get; set; }

        private short[] RenderOscillator(IOscillator osc)
        {
            short[] data = new short[_numSamples];
            double angle = (Math.PI * 2 * osc.OscFrequency) / (format.dwSamplesPerSec * format.wChannels);
            switch (osc.Wavetype)
            {
                case WaveType.Sinus:
                    for (int i = 0; i < _numSamples; i++)
                    {
                        data[i] = Convert.ToInt16(osc.OscAmplitude * Math.Sin(angle * i));
                    }
                    break;
                case WaveType.Square:
                    for (int i = 0; i < _numSamples; i++)
                    {
                        if (Math.Sin(angle * i) > 0) data[i] = Convert.ToInt16(osc.OscAmplitude);
                        else data[i] = Convert.ToInt16(-osc.OscAmplitude);
                    }
                    break;
                case WaveType.Saw:
                    {
                        int samplesPerPeriod = Convert.ToInt32(format.dwSamplesPerSec / (osc.OscFrequency / format.wChannels));
                        short sampleStep = Convert.ToInt16((osc.OscAmplitude * 2) / samplesPerPeriod);
                        short tempSample = 0;

                        int i = 0;
                        int totalSamplesWritten = 0;
                        while (totalSamplesWritten < _numSamples)
                        {
                            tempSample = (short)-osc.OscAmplitude;
                            for (i = 0; i < samplesPerPeriod && totalSamplesWritten < _numSamples; i++)
                            {
                                tempSample += sampleStep;
                                data[totalSamplesWritten] = tempSample;
                                totalSamplesWritten++;
                            }
                        }
                    }
                    break;
                case WaveType.Noise:
                    {
                        Random rnd = new Random();
                        for (int i = 0; i < _numSamples; i++)
                        {
                            data[i] = Convert.ToInt16(rnd.Next((int)-osc.OscAmplitude, (int)osc.OscAmplitude));
                        }
                    }
                    break;
                case WaveType.None:
                    data = null;
                    break;
            }
            return data;
        }

        private void Save(string target)
        {
            FileStream fileStream = new FileStream(target, FileMode.Create);

            using (BinaryWriter writer = new BinaryWriter(fileStream))
            {
                // Write the header
                writer.Write(header.sGroupID.ToCharArray());
                writer.Write(header.dwFileLength);
                writer.Write(header.sRiffType.ToCharArray());

                // Write the format chunk
                writer.Write(format.sChunkID.ToCharArray());
                writer.Write(format.dwChunkSize);
                writer.Write(format.wFormatTag);
                writer.Write(format.wChannels);
                writer.Write(format.dwSamplesPerSec);
                writer.Write(format.dwAvgBytesPerSec);
                writer.Write(format.wBlockAlign);
                writer.Write(format.wBitsPerSample);

                // Write the data chunk
                writer.Write(data.sChunkID.ToCharArray());
                writer.Write(data.dwChunkSize);
                foreach (short dataPoint in data.shortArray)
                {
                    writer.Write(dataPoint);
                }

                writer.Seek(4, SeekOrigin.Begin);
                uint filesize = (uint)writer.BaseStream.Length;
                writer.Write(filesize - 8);
            }
        }

        private void RenderWav(string targetfile = null)
        {
            short[] oscdata;
            if (targetfile == null) targetfile = _targetfile;
            var workosc = (from osc in _oscillators where osc.Wavetype != WaveType.None select osc).ToArray();
            data.shortArray = new short[_numSamples];
            foreach (var oscillator in workosc)
            {
                oscdata = RenderOscillator(oscillator);
                if (oscdata == null) continue;
                for (int i = 0; i < _numSamples; i++)
                {
                    int sum = data.shortArray[i] + oscdata[i];
                    data.shortArray[i] = (short)(sum / workosc.Length);
                }
            }
            data.dwChunkSize = (uint)(data.shortArray.Length * (format.wBitsPerSample / 8));
            Save(targetfile);
        }

        private void GraphWaveform()
        {
            if (DrawArea == null) return;
            DrawArea.Children.Clear();

            double canvasHeight = DrawArea.ActualHeight;
            double canvasWidth = DrawArea.ActualWidth;

            int observablePoints = 1764;

            double xScale = canvasWidth / observablePoints;
            double yScale = (canvasHeight /
                (double)(MAX_AMPLITUDE * 2));

            Polyline graphLine = new Polyline();
            graphLine.Stroke = Brushes.Black;
            graphLine.StrokeThickness = 1;


            for (int i = 0; i < observablePoints; i++)
            {

                graphLine.Points.Add(
                    new Point(i * xScale, (canvasHeight / 2) - (data.shortArray[i] * yScale)));
            }

            DrawArea.Children.Add(graphLine);
        }

        public void Play()
        {
            foreach (var osc in _oscillators) osc.Running = true;
            RenderWav();
            GraphWaveform();
            _sndp.SoundLocation = _targetfile;
            _sndp.PlayLooping();
        }

        public void SavetoFile(string filename)
        {
            RenderWav(filename);
        }

        public void Stop()
        {
            _sndp.Stop();
            foreach (var osc in _oscillators) osc.Running = false;
        }

        protected virtual void Dispose(bool native)
        {
            if (File.Exists(_targetfile)) File.Delete(_targetfile);
            if (_sndp != null)
            {
                _sndp.Dispose();
                _sndp = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
