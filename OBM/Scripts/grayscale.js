function changeGrayscale() {
    var body = document.body;
    console.log(body);
    var check = body.style['filter'];
    if (check != '') {
        body.style['filter'] = '';
    }
    else {
        body.style['filter'] = 'grayscale(1)';
        if (!body.style['filter']) {
            body.style['-webkit-filter'] = 'grayscale(1)';
            body.style['filter'] = 'grayscale(1)';
        }                
    }
    console.log(body);
}