﻿// Toggle visibility ON or OFF for Logout 
function toggleVisibility(passedElementId, togglevalue) {

    // access passed element
    var e = document.getElementById(passedElementId);

    // read visibility status
    //assign inverse visibility status
    if (togglevalue == 'on') {
        e.style.display = 'block';
    } else if (togglevalue == 'off') {
        e.style.display = 'none';
    } else {
        e.style.display = 'block';
    }

}


//automatic visibility toggle for accesspoint settings and district settings.
function toggleVisibilityauto(passedElementId) {
    // access passed element
    var e = document.getElementById(passedElementId);

    if (e.style.display == "block") {
        e.style.display = 'none';

    } else if (e.style.display == "none") {
        e.style.display = 'block';

    }
    else {
        e.style.display = "block";

    }

}