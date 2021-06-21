// Dynamically change the favicon
const favicon = document.querySelectorAll('[rel="icon"]')[0];
favicon.setAttribute('href', '/swagger/favicon.ico');

// Set the nav image as a link to the blog
setTimeout(SetHref, 1000);

function SetHref() {
    document.querySelector(".topbar-wrapper a").setAttribute("href", "/Home/Index/");
}


