(function ($) {


    function parseTemplate() {
        updateStatus('waiting');

        function onSuccess(resp) {
            if (resp.Success) { updateStatus('success'); }
            else { updateStatus('fail'); }

            showMessages(resp.Messages);
            showGeneratedCode(resp.GeneratedCode);
        }

        function onError(err) {
            updateStatus('fail');
            showMessages(err);
            showGeneratedCode(' [[**** PARSE ERROR ****]] ');
        }

        $('#generated-code').html('');

        $.ajax({
            url: 'razorpad/parse',
            data: JSON.stringify({ 'Template': $('#template').val() }),
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            type: 'post',
            success: onParseSuccess,
            error: onParseError
        });
    } // END parseTemplate()

    function executeTemplate() {
        updateStatus('waiting');

        $('#template-output').html('');

        $.ajax({
            url: 'razorpad/execute',
            data: JSON.stringify({ 'Template': $('#template').val(), "Parameters": [] }),
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            type: 'post',
            success: onExecuteSuccess,
            error: onExecuteError
        });
    } // END executeTemplate()


    function onParseSuccess(resp) {
        if (resp.Success) { updateStatus('success'); }
        else { updateStatus('fail'); }

        showMessages(resp.Messages);
        showGeneratedCode(resp.GeneratedCode);
    }

    function onParseError(err) {
        updateStatus('fail');
        showMessages(err);
        showGeneratedCode(' [[**** PARSE ERROR ****]] ');
    }

    function onExecuteSuccess(resp) {
        onParseSuccess(resp);
        $('#template-output').html(resp.TemplateOutput);
    }

    function onExecuteError(resp) {
        $('#template-output').html(' [[**** EXECUTION ERROR ****]] ');
        onParseError(resp);
    }


    function updateStatus(status) {
        $('#template-container').attr('class', status);
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

    function showGeneratedCode(code) {
        $('#generated-code').html(code);
    }


    $('#generate-code').click(parseTemplate);
    $('#execute').click(executeTemplate);

})(jQuery);