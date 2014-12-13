using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTools.classes
{
    public class QuineMcclusky
    {
        #region "Support Classes"

        private class ImplicantRelationship
        {
            public Implicant a { get; set; }
            public Implicant b { get; set; }

            public ImplicantRelationship(Implicant first, Implicant second)
            {
                a = first;
                b = second;
            }
        }

        private class ImplicantCollection : List<Implicant> { }

        private class ImplicantRelationshipCollection : List<ImplicantRelationship> { }

        private class Implicant
        {
            public string Mask { get; set; } //number mask.
            public List<int> Minterms { get; set; }

            public Implicant()
            {
                Minterms = new List<int>(); //original integers in group.
            }

            public string ToChar(int length, bool lsba = false, bool negate = false)
            {
                string strFinal = string.Empty;
                string mask = Mask;

                while (mask.Length != length)
                    mask = "0" + mask;

                if (!lsba)
                {
                    for (int i = 0; i < mask.Length; i++)
                    {
                        if (negate)
                        {
                            if (mask[i] == '0') strFinal += Convert.ToChar(65 + i) + "+";
                            else if (mask[i] == '1') strFinal += Convert.ToChar(65 + i) + "'+";
                        }
                        else
                        {
                            if (mask[i] == '0')  strFinal += Convert.ToChar(65 + i) + "'";
                            else if (mask[i] == '1') strFinal += Convert.ToChar(65 + i);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < mask.Length; i++)
                    {
                        if (negate)
                        {
                            if (mask[i] == '0') strFinal += Convert.ToChar((65 + (length - 1)) - i) +"+";
                            else if (mask[i] == '1') strFinal += Convert.ToChar((65 + (length - 1)) - i) + "'+";
                            if (i != mask.Length - 1) strFinal += "+";
                        }
                        else
                        {
                            if (mask[i] == '0') strFinal += Convert.ToChar((65 + (length - 1)) - i) + "'";
                            else if (mask[i] == '1') strFinal += Convert.ToChar((65 + (length - 1)) - i);
                        }
                    }
                }
                if (negate) return "(" + strFinal.Remove(strFinal.Length - 1, 1) + ")";
                return strFinal;
            }
        }

        #endregion


        #region "Utility Functions"

        /*
         * Returns length balanced versions of a string.
         * If given a=010 and b=10101 it will return 00010 and 10101.
         */
        private static void GetBalanced(ref string a, ref string b)
        {
            while (a.Length != b.Length)
            {
                if (a.Length < b.Length)
                    a = "0" + a;
                else
                    b = "0" + b;
            }
        }

        /*
         * Returns the number of binary differences when passed two integers as strings.
         */
        private static int GetDifferences(string a, string b)
        {
            GetBalanced(ref a, ref b);

            int differences = 0;

            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    differences++;

            return differences;
        }

        /*
         * Retreives the number of '1' characters in a string.
         */
        private static int GetOneCount(string a)
        {
            int count = 0;

            foreach (char c in a.ToCharArray())
                if (c == '1')
                    count++;

            return count;
        }

        /*
         * Calculates a mask given two input strings.
         * For example when passed a=1001 and b=1101
         * will return 1-01.
         */
        private static string GetMask(string a, string b)
        {
            GetBalanced(ref a, ref b);

            string final = string.Empty;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    final += '-';
                else
                    final += a[i];
            }

            return final;
        }
        #endregion


        #region "Core Functions"

        /*
         * Simplifies a givenset of implicants.
         */
        private static bool Simplify(ref ImplicantCollection implicants)
        {
            /*
             * Group by number of 1's and determine relationships by comparing.
             */
            var groups = (from i in Group(implicants) orderby i.Key select i).ToDictionary(i => i.Key, i => i.Value);
            ImplicantRelationshipCollection relationships = new ImplicantRelationshipCollection();
            for (int i = 0; i < groups.Keys.Count; i++)
            {
                if (i == (groups.Keys.Count - 1)) break;

                ImplicantCollection thisGroup = groups[groups.Keys.ElementAt(i)];
                ImplicantCollection nextGroup = groups[groups.Keys.ElementAt(i + 1)];

                var q = from a in thisGroup from b in nextGroup where GetDifferences(a.Mask, b.Mask) == 1 select new ImplicantRelationship(a, b);
                relationships.AddRange(q);
            }

            /*
             * For each relationship, find the affected minterms and remove them.
             * Then add a new implicant which simplifies the affected minterms.
             */
            foreach (ImplicantRelationship r in relationships)
            {
                ImplicantCollection rmList = new ImplicantCollection();

                foreach (Implicant m in implicants)
                {
                    if (r.a.Equals(m) || r.b.Equals(m)) rmList.Add(m);
                }

                foreach (Implicant m in rmList) implicants.Remove(m);

                Implicant newImplicant = new Implicant();
                newImplicant.Mask = GetMask(r.a.Mask, r.b.Mask);
                newImplicant.Minterms.AddRange(r.a.Minterms);
                newImplicant.Minterms.AddRange(r.b.Minterms);

                bool exist = false;
                foreach (Implicant m in implicants)
                {
                    if (m.Mask == newImplicant.Mask) exist = true;
                }

                if (!exist) //Why am I getting dupes?
                    implicants.Add(newImplicant);
            }

            //Return true if simplification occurred, false otherwise.
            return !(relationships.Count == 0);
        }

        /*
         * Populates a matrix based on a given set of implicants and minterms.
         */
        private static void PopulateMatrix(ref bool[,] matrix, ImplicantCollection implicants, int[] inputs)
        {
            for (int m = 0; m < implicants.Count; m++)
            {
                int y = implicants.IndexOf(implicants[m]);

                foreach (int i in implicants[m].Minterms)
                    for (int index = 0; index < inputs.Length; index++)
                        if (i == inputs[index])
                            matrix[y, index] = true;
            }
        }

        /*
         * Groups binary numbers based on 1's.
         * Stores group in a hashtable associated with a list (bucket) for each group.
         */
        private static Dictionary<int, ImplicantCollection> Group(ImplicantCollection implicants)
        {
            Dictionary<int, ImplicantCollection> group = new Dictionary<int, ImplicantCollection>();
            foreach (Implicant m in implicants)
            {
                int count = GetOneCount(m.Mask);

                if (!group.ContainsKey(count))
                    group.Add(count, new ImplicantCollection());

                group[count].Add(m);
            }

            return group;
        }

        /*
         * Retreives the final simplified expression in readable format.
         */
        private static string GetFinalExpression(ImplicantCollection implicants, bool lsba = false, bool negate = false)
        {
            int longest = 0;
            string final = string.Empty;

            foreach (Implicant m in implicants)
                if (m.Mask.Length > longest)
                    longest = m.Mask.Length;

            for (int i = implicants.Count - 1; i >= 0; i--)
            {
                if (negate) final += implicants[i].ToChar(longest, lsba, negate) + " & ";
                else final += implicants[i].ToChar(longest, lsba, negate) + " + ";
            }

            string ret = (final.Length > 3 ? final.Substring(0, final.Length - 3) : final);
            switch (ret)
            {
                case " + ":
                    return "1";
                case "":
                    return "0";
                default:
                    return ret;
            }
        }

        private static bool ContainsSubList(List<int> list, List<int> OtherList)
        {
            bool ret = true;
            foreach (var item in OtherList)
            {
                if (!list.Contains(item))
                {
                    ret = false;
                    break;
                }
            }
            return ret;
        }

        private static bool ContainsAtleastOne(List<int> list, List<int> OtherList)
        {
            bool ret = false;
            foreach (var item in OtherList)
            {
                if (list.Contains(item))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        /*
         * Selects the smallest group of implicants which satisfy the equation from the matrix.
         */
        private static ImplicantCollection SelectImplicants(ImplicantCollection implicants, int[] inputs)
        {
            List<int> lstToRemove = new List<int>(inputs);
            ImplicantCollection final = new ImplicantCollection();
            int runnumber = 0;
            while (lstToRemove.Count != 0)
            {
                //Implicant[] weightedTerms = WeightImplicants(implicants, final, lstToRemove);
                foreach (Implicant m in implicants)
                {
                    bool add = false;

                    if (ContainsSubList(lstToRemove, m.Minterms))
                    {
                        add = true;
                        if (lstToRemove.Count < m.Minterms.Count) break;
                    }
                    else add = false;

                    if (((lstToRemove.Count <= m.Minterms.Count) && add == false) || runnumber > 5)
                    {
                        if ( ContainsAtleastOne(lstToRemove, m.Minterms) && runnumber > 5) add = true;
                    }
                    
                    if (add)
                    {
                        final.Add(m);
                        foreach (int r in m.Minterms) lstToRemove.Remove(r);
                    }
                }
                foreach (var item in final) implicants.Remove(item); //ami benne van már 1x, az még 1x ne
                ++runnumber;
            }

            return final;
        }
        
        public static string GetSimplified(IEnumerable<LogicItem> List, int variables, bool hazardsafe = false, bool lsba = false, bool negate = false)
        {
            ImplicantCollection implicants = new ImplicantCollection();

            var items = (from i in List where i.Checked == true || i.Checked == null orderby i.Index ascending select i.Index).ToArray();
            var careminterms = (from i in List where i.Checked == true orderby i.Index ascending select i.Index).ToArray();



            foreach (var item in items)
            {
                Implicant m = new Implicant();
                m.Mask = LogicItem.GetBinaryValue(item, variables);
                m.Minterms.Add(item);
                implicants.Add(m);
            }

            //int count = 0;
            while (Simplify(ref implicants))
            {
                //Populate a matrix.
                bool[,] matrix = new bool[implicants.Count, items.Length]; //x, y
                PopulateMatrix(ref matrix, implicants, items);
            }
            ImplicantCollection selected;
            if (hazardsafe) selected = implicants;
            else selected = SelectImplicants(implicants, careminterms);
            return GetFinalExpression(selected, lsba, negate);
        }

        #endregion
    }
}
