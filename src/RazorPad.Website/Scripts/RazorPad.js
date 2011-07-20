(function ($) {

    function parseTemplate() {
        $.ajax({
            url: 'razorpad/parse',
            data: JSON.stringify({ 'Template': $('#template').val() }),
            success: onParseSuccess,
            error: onParseError
        });
    } // END parseTemplate()

    function executeTemplate() {
        var model = getModel();

        $.ajax({
            url: 'razorpad/execute',
            data: JSON.stringify({ 'Template': $('#template').val(), "Model": JSON.stringify(model) }),
            success: function (resp) {
                onParseSuccess(resp);
                $('#template-output').html(resp.TemplateOutput);
            },
            error: function (resp) {
                onParseError(resp);
                $('#template-output').html(' [[**** EXECUTION ERROR ****]] ');
            }
        });
    } // END executeTemplate()


    function onParseError(err) {
        updateStatus('fail');
        showMessages([{ Kind: 'Error', Text: JSON.stringify(err)}]);
        $('#generated-code').html(' [[**** PARSE ERROR ****]] ');
    }

    function onParseSuccess(resp) {
        if (resp.Success) { updateStatus('success'); }
        else { updateStatus('fail'); }

        showMessages(resp.Messages);
        $('#generated-code').html(resp.GeneratedCode);

        var div = document.createElement('div');
        div.appendChild(document.createTextNode(resp.ParsedDocument));
        $('#parser-results').html(div.innerHTML);
    }


    function showMessages(messages) {
        var messagesList = $('#messages');
        messagesList.html('');

        $.each(messages, function (idx, message) {
            $('<li/>')
                .addClass(message.Kind)
                .html($('<pre/>').html(message.Text))
                .appendTo(messagesList);
        });
    }

    function updateStatus(status) {
        $('#template-container').attr('class', status);
    }


    $('#generate-code').click(parseTemplate);
    $('#execute').click(executeTemplate);




    $.ajaxSetup({
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        type: 'post'
    });

    $.ajaxPrefilter(function (options, originalOptions, jqXHR) {
        updateStatus('waiting');
        $('#template-output').html('');
        $('#generated-code').html('');
    });

})(jQuery);