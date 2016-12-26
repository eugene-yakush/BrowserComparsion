function LoadComparsion(caption, path)
{
    var fullpath = 'Content/Samples/' + path;
    //alert('' + caption + ':' + fullpath);

    document.getElementById('sample_title').innerText = caption;
    document.getElementById('sample_body').innerHTML = '<img src="Content/Images/ajax-loader.gif">';

    $.ajax({
        url: fullpath,
        success: function(data)
        {
            var hl = Prism.highlight(data, Prism.languages.csharp);
            document.getElementById('sample_body').innerHTML = hl;
        },
        error: function(a,b,c) { document.getElementById('sample_body').innerHTML = "Unable to find source"; }
    });
}