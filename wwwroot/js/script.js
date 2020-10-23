console.log(uinput);
var depData = [];
jsonString.forEach(element => depData.push([element]));

google.charts.load('current', {packages:['wordtree']});
google.charts.setOnLoadCallback(drawChart);

function drawChart() {
  var data = google.visualization.arrayToDataTable(depData);

  var options = {
    wordtree: {
      format: 'implicit',
      word: uinput
    }
  };

  var chart = new google.visualization.WordTree(document.getElementById('wordtree_basic'));
  chart.draw(data, options);
}