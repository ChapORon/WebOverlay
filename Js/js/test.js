function onBlinkLoad() {
	var b = document.getElementById('blink');
    setInterval(function() {
        b.style.display = (b.style.display == 'none' ? '' : 'none');
    }, 1000);
}