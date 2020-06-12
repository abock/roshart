var itemsOffset = 0;
var itemsLimit = 10;
var animElement = null;
var loading = false;
var canLoadMore = true;

function loadItems () {
  if (loading || !canLoadMore) {
    return;
  }

  loading = true;

  var spinner = new Spinner ({
    lines: 13, // The number of lines to draw
    length: 36, // The length of each line
    width: 13, // The line thickness
    radius: 35, // The radius of the inner circle
    corners: 1, // Corner roundness (0..1)
    rotate: 72, // The rotation offset
    direction: 1, // 1: clockwise, -1: counterclockwise
    color: "#E80C7A", // #rgb or #rrggbb
    speed: 1.5, // Rounds per second
    trail: 60, // Afterglow percentage
    shadow: true, // Whether to render a shadow
    hwaccel: false, // Whether to use hardware acceleration
    className: "spinner", // The CSS class to assign to the spinner
    zIndex: 2e9, // The z-index (defaults to 2000000000)
    top: "auto", // Top position relative to parent in px
    left: "auto" // Left position relative to parent in px
  });

  spinner.spin ();
  var spinnerContainer = document.getElementById ("loading");
  spinnerContainer.appendChild (spinner.el);

  $.get ("/api/query?offset="
    + itemsOffset + "&limit=" + itemsLimit, function (data) {
    var items = $("#items");
    var urls = data.split ("\n");
    for (var i = 0, length = urls.length; i < length; i++) {
      var url = urls[i];

      if (!url)
        continue;
      else if (url.indexOf ("https://") === 0)
        url = url.substr (6);
      else if (url.indexOf ("http://") === 0)
        url = url.substr (5);
      else
        continue;

      var th_url = url + "?thumbnail=1"

      var item = $(
        "<div class=\"item\">" + 
        // "<img class=\"thumb\" src=\"" + th_url + "\" />" +
        "<img class=\"anim\" src=\"" + url + "\" />" +
        "</div>" +
        "<br />"
      );

      items.append (item);
      
      /*item.children ("img.thumb").mouseover (function (e) {
        if (animElement) {
          animElement.css ("visibility", "hidden");
        }
        
        animElement = $(this).next ();
        animElement.css ("visibility", "visible");
        return false;
      });

      // item.children ("img.anim").mouseout (function (e) {
      //   $(this).css ("visibility", "hidden");
      //   animElement = null;
      //   return false;
      // });*/

      item.children ("img").click (function () {
        window.open ("http://amzn.to/18KXfZJ");
      });
    }

    // items.append ($('<iframe src="http://rcm-na.amazon-adsystem.com/e/cm?t=catoverflow-20&o=1&p=9&l=ez&f=ifr&f=ifr" width="180" height="150" scrolling="no" marginwidth="0" marginheight="0" border="0" frameborder="0" style="border:none;"></iframe>'));

    if (urls.length < itemsLimit) {
      itemsOffset += urls.length;
      canLoadMore = false;
    } else {
      itemsOffset += itemsLimit;
    }

    loading = false;
    spinnerContainer.removeChild (spinner.el);
    spinner.stop ();
  });
}

$(document).ready (function () {
  var items = $("#items");

  loadItems ();

  $(window).scroll (function () {
    var w = $(this);
    if (w.scrollTop () + w.height () > $(document).height () - 100) {
      loadItems ();
    }
  });
});
