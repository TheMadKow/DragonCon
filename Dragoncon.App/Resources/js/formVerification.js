function CreateDefaultPostData() {
    var postData = {};
    postData["__RequestVerificationToken"] = $('input[name=__RequestVerificationToken]').val();
    return postData;

}