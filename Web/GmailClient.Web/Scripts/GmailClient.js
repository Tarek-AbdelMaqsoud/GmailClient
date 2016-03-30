$(document).ready(function () {
    $("#ComposeLink").click(function () {
        $("#modalCompose").modal();
    });
    $("#PrevButton").click(function () {
        
        var temp = parseInt($("#pageNumber").val()) - 1;
        window.location.href = "/Mail/ListAll?pageNumber=" + temp;
    });
    $("#NextButton").click(function () {
        var temp = parseInt($("#pageNumber").val()) + 1;
        window.location.href = "/Mail/ListAll?pageNumber=" + temp;
    });
    function OnSuccess(Response, text) {
        $("#errorSpan").addClass("alert-success");
        $("#errorSpan").removeClass("alert-danger");
        $("#errorSpan").html("<strong>Success!</strong> " + Response.message);
        $("#errorSpan").removeClass("hide");
        $("#errorSpan").addClass("show");
        $("#modalCompose").addClass("hide");
        $("#modalCompose").removeClass("show");
        $(".modal-backdrop").hide();
    }

    function OnError(Request, status, error) {
        $("#errorSpan").addClass("alert-danger");
        $("#errorSpan").removeClass("alert-success");
        $("#errorSpan").html("<strong>Error!</strong> " + Request.message);
        $("#errorSpan").removeClass("hide");
        $("#errorSpan").addClass("show");
        $("#modalCompose").addClass("hide");
        $("#modalCompose").removeClass("show");
        $(".modal-backdrop").hide();
    }
    $("#sendMail").click(function () {
        var model =
            {
                ToAsCsv: $("#inputTo").val(),
                Subject: $("#inputSubject").val(),
                Body: $("#inputBody").val(),
                FromEmail: "@ViewBag.EmailId",
            }

        $.ajax({
            url: '/Mail/Compose/',
            type: 'POST',
            data: JSON.stringify(model),
            dataType: 'json',
            processData: false,
            contentType: 'application/json; charset=utf-8',
            success: OnSuccess,
            error: OnError,
            timeout: 30000
        });
    });
});
function OnSuccess(Response, text) {
    $("#errorSpan").addClass("alert-success");
    $("#errorSpan").removeClass("alert-danger");
    $("#errorSpan").html("<strong>Success!</strong> " + Response.message);
    $("#errorSpan").removeClass("hide");
    $("#errorSpan").addClass("show");
    $("#modalCompose").addClass("hide");
    $("#modalCompose").removeClass("show");
}

function OnError(Request, status, error) {
    $("#errorSpan").addClass("alert-danger");
    $("#errorSpan").removeClass("alert-success");
    $("#errorSpan").html("<strong>Error!</strong> " + Request.message);
    $("#errorSpan").removeClass("hide");
    $("#errorSpan").addClass("show");
    $("#modalCompose").addClass("hide");
    $("#modalCompose").removeClass("show");
}

$("#btnDelete").click(function () {
    $("#errorSpan").removeClass("show");
    $("#errorSpan").addClass("hide");
    if ($('input.MessageCheckBox:checked').length > 0) {
        var csvUids = '';
        $('input.MessageCheckBox:checked').each(function () {
            csvUids = csvUids + ',' + $(this).prop('id');
        });
        csvUids = csvUids.replace(/(^,)|(,$)/g, "");
        $.ajax({
            url: '/Mail/Delete',
            type: 'POST',
            data: "{csEmailUids:'" + csvUids + "'}",
            dataType: 'json',
            processData: false,
            contentType: 'application/json; charset=utf-8',
            success: OnSuccess,
            error: OnError,
            timeout: 30000
        });
    }
    else {
        var req =
            {
                message: "Please select at least one email to delete it!"
            };
        OnError()
    }
});

function SelectAll() {
    if ($('#CheckAll:checked').length > 0) {
        $('.MessageCheckBox').prop('checked', true);
    }
    else {
        $('.MessageCheckBox').prop('checked', false);
    }
}
