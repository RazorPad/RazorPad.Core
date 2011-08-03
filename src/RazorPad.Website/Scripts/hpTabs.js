(function ($) {
    $.fn.extend({
        hpTabs: function (options) {
            var defaults = {},
                    options = $.extend(defaults, options);

            var ShowTab = function (lnk) {
                var $lnk = $(lnk),
    	            $panel = $($lnk.attr("href"));
                $lnk.siblings("a").removeClass("selected");
                $panel.siblings("div").hide();
                $lnk.addClass("selected");
                $panel.show();
            };

            return this.each(function () {
                var o = options;
                var $this = $(this);
                $this.find(".tabBar > a").click(function () {
                    ShowTab(this);
                    return false;
                });
                ShowTab($this.find(".tabBar > a")[0]);
            });
        }
    });
} (jQuery))