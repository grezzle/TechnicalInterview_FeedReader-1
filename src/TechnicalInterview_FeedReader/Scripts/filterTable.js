/* jquery for search nad filter table start*/

/*$.noConflict();*/
$(document).ready(function () {
    // Write on keyup event of keyword input element
    $("#search").keyup(function () {
        // When value of the input is not blank
        var term = $(this).val()
        if (term != "") {
            // Show only matching TR, hide rest of them
            $("#rounded-corner tbody>tr").hide();
            $("#rounded-corner td").filter(function () {
                return $(this).text().toLowerCase().indexOf(term) > -1
            }).parent("tr").show();
        }
        else {
            // When there is no input or clean again, show everything back
            $("#rounded-corner tbody>tr").show();
        }
    });
});

/* jquery for search nad filter table ends*/