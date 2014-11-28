function EndsWith(str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

function RenderDB(database, target, searchstring) {
    var html = "";
    for (var key in database) {
        if (searchstring == "" || database[key].toLowerCase().indexOf(searchstring.toLowerCase()) > -1) {
            html += "<a class=\"fancybox button big button-green\" href=\"" + key + "\" rel=\"gallery\" title=\"" + database[key] + "\">" + database[key] + "</a>";
        }
    }
    $(target).html(html);
    $(".fancybox").fancybox({
        prevEffect: 'elastic',
        nextEffect: 'elastic',
        closeBtn: false,
        helpers: {
            title: { type: 'inside' },
            buttons: {}
        }
    });
}

function Youtube(iframe, videourl)
{
        $(iframe).attr('src', 'http://www.youtube.com/embed/'+videourl);
        ScrollToAnchor("playertop");
}