function trimText(str, wordCount) {
    var strArray = str.split(' ');
    var subArray = strArray.slice(0, wordCount);
    var result = subArray.join(" ");
    return result + '...';
}

var str = $('td .more').text();
var result = trimText(str, 20);
$('td .more').text(result);