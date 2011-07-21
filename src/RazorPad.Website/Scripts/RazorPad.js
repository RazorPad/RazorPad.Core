(function ($) {

    function executeTemplate() {
        var model = getModel();

        $.ajax({
            url: 'razorpad/execute',
            data: JSON.stringify({ 'Template': $('#template').val(), "Model": JSON.stringify(model) }),
            success: function (resp) {
                onParseSuccess(resp);
                $('#rendered-output').html(resp.TemplateOutput);
                $('#template-output').text(resp.TemplateOutput);
            },
            error: function (resp) {
                onParseError(resp);
                var message = ' [[**** EXECUTION ERROR ****]] \r\n' + JSON.stringify(resp);
                $('#rendered-output').text(message);
                $('#template-output').text(message);
            }
        });
    } // END executeTemplate()

    function getModel() {
        // TODO: Build model from user input
        var model = {
            "Name": "Frank Sinatra",
            "Address": "1234 Stardust Ln, Las Vegas, NV",
            "Birthday": new Date(1960, 3, 12),
            "UserID": 9989
        };

        return model;
    }

    function onParseError(err) {
        updateStatus('fail');
        showMessages([{ Kind: 'Error', Text: JSON.stringify(err)}]);
        $('#generated-code').html(' [[**** PARSE ERROR ****]] ');
    } // END onParseError()

    function onParseSuccess(resp) {
        if (resp.Success) { updateStatus('success'); }
        else { updateStatus('fail'); }

        showMessages(resp.Messages);
        //$('#generated-code').empty().append(prettyPrintOne(resp.GeneratedCode, 'cs', true));
        $('#generated-code').empty().text(resp.GeneratedCode);

        //$('#parser-results').empty().append(prettyPrintOne(div.innerHTML, 'html', true));
        $('#parser-results').empty().text(resp.ParsedDocument);

        SyntaxHighlighter.highlight();
    } // END onParseSuccess()

    function parseTemplate() {
        $.ajax({
            url: 'razorpad/parse',
            data: JSON.stringify({ 'Template': $('#template').val() }),
            success: onParseSuccess,
            error: onParseError
        });
    } // END parseTemplate()

    function showMessages(messages) {
        var messagesList = $('#messages');
        messagesList.html('');

        $.each(messages, function (idx, message) {
            $('<li/>')
                .addClass(message.Kind)
                .html($('<pre/>').html(message.Text))
                .appendTo(messagesList);
        });
    } // END showMessages()

    function updateStatus(status) {
        $('#template-container').attr('class', status);
    }


    // $('#generate-code').click(parseTemplate);
    $('#execute')
        .click(executeTemplate)
        .ajaxStart(function () {
            updateStatus('waiting');
            $('#template-output').html('');
            $('#generated-code').html('');
        });


    $.ajaxSetup({
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        type: 'post'
    });

})(jQuery);