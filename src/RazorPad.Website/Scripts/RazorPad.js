(function ($) {

	function executeTemplate() {
		var model = getModel();

		$.ajax({
			url: 'razorpad/execute',
			data: JSON.stringify({ 'Template': $('#template').val(), "Model": JSON.stringify(model) }),
			success: function (resp) {
				onParseSuccess(resp);
				showRenderedTemplateOutput(resp.TemplateOutput);
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
		$('#generated-code-container').empty().append($('<pre id="generated-code" class="brush: csharp"></pre>').text(resp.GeneratedCode));

		$('#parser-result-container').empty().append($('<pre id="parser-results" class="brush: html"></pre>').text(resp.ParsedDocument));
		SyntaxHighlighter.highlight({ toolbar: false });
	} // END onParseSuccess()


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

	function showRenderedTemplateOutput(templateOutput) {
		var iframe = $('iframe', '#rendered-output-container');

		if (!iframe.get(0))
			iframe = $('<iframe>').appendTo('#rendered-output-container');

		iframe.contents().find('body').html(templateOutput);
	}

	function updateStatus(status) {
		$('#template-container').attr('class', status);
	}


	$('#execute')
		.click(executeTemplate)
		.ajaxStart(function () {
			updateStatus('waiting');
			$('#template-output').text('');
			$('#generated-code').text('');
		});


	$.ajaxSetup({
		contentType: "application/json; charset=utf-8",
		dataType: 'json',
		type: 'post'
	});


	$('#template').focus(function () {

		$(this).filter(function () {

			// We only want this to apply if there's not
			// something actually entered
			return $(this).val() === "" || $(this).val() === "Razor Markup Here";

		}).removeClass("watermarkOn").val("");

	});

	// Define what happens when the textbox loses focus
	// Add the watermark class and default text
	$('#template').blur(function () {

		$(this).filter(function () {

			// We only want this to apply if there's not
			// something actually entered
			return $(this).val() === "";

		}).addClass("watermarkOn").val("Razor Markup Here");

	});


})(jQuery);