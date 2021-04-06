
$(function () {
    $('.widget').widgster();
    function pageLoad(){
        $('.selectpicker').selectpicker();
    }
    pageLoad();
    SingApp.onPageLoad(pageLoad);
});