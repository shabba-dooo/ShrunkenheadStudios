
if (website === undefined) { var website = {}; }

website.SmoothScroll = function () {
    $('a[href*=#]:not([href=#])').click(function () {
        if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') || location.hostname == this.hostname) {
            var target = $(this.hash);
            target = target.length ? target : $('[ids=' + this.hash.slice(1) + ']');
            if (target.length) {
                $(".menu").toggleClass("menu-open");
                $('html,body').animate({
                    scrollTop: target.offset().top - 80
                }, 1000);
                return false;
            }
        }
    });

    jQuery(window).load(function () {
        function goToByScroll(id) {
            jQuery('html,body').animate({ scrollTop: $("#" + id).offset().top - 80 }, 1000);
        }

        if (window.location.hash != '') {
            goToByScroll(window.location.hash.substr(1));
        };
    });
}

website.CalculateDateAge = function () {
	 dob = new Date("1989-03-12");
     var today = new Date();
     var age = Math.floor((today-dob) / (365.25 * 24 * 60 * 60 * 1000));

     $(document).ready(function() {
     $("#age").text( age );
    });
}

website.ResponsiveNav = function () {

    $("a[href=#menu-expand]").click(function (e) {
        $(".menu").toggleClass("menu-open");
        e.preventDefault();
    });
}

// reusable ready function
website.ready = function () {
    var self = this;

    self.SmoothScroll();
    self.CalculateDateAge();
    self.ResponsiveNav();
};

$(function () {
    website.ready();
});