anime.timeline({ loop: false })
    .add({
        targets: '.oktitle .wordsok',
        scale: [12, 1.2],
        opacity: [0, 1],
        easing: "easeOutCirc",
        duration: 1000,
        delay: (el, i) => 1000 * i
    })
    .add({
        targets: '.bmtitle .wordsbm',
        scale: [10, 2],
        opacity: [0, 1],
        easing: "easeOutCirc",
        duration: 1000,
        delay: (el, i) => 1000 * i
    });


var textWrapper = document.querySelector('.titledesc');
textWrapper.innerHTML = textWrapper.textContent.replace(/\S/g, "<span class='letter'>$&</span>");

anime.timeline({ loop: false })
    .add({
        targets: '.titledesc .letter',
        opacity: [0, 1],
        easing: "easeInOutQuad",
        duration: 2250,
        delay: (el, i) => 100 * (i + 1)
    });

$(document).ready(function () {
    $('#eventtable').DataTable({
        "ordering": false
    });
    $('.dataTables_info').addClass('text-light');
    $('label').addClass('text-light');
});