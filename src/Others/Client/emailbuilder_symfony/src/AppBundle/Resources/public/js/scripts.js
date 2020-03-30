var monthNames = ["jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec"];
var monthNums = ["01","02","03","04","05","06","07","08","09","10","11","12"];
var monthNamesFull = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
var d = new Date();
var todaysDate = d.getDate();
var todaysWeekday = d.getDay();
var monthAbbr = monthNames[d.getMonth()];
var monthNum = monthNums[d.getMonth()];
var nextMonthAbbr = monthNames[d.getMonth() + 1];
var fullMonthName = monthNamesFull[d.getMonth()];
var year = d.getFullYear();
//var startDate = getNearestTuesday(todaysDate, todaysWeekday);
//var nextFriday = getNearestFriday(todaysDate,todaysWeekday);
//var endDate = Number(startDate + 5);
var yearMonDay = year+"-"+(monthNums[d.getMonth()])+"-"+todaysDate;
var monDayYear = fullMonthName + " " + todaysDate + ", " + year;

function updateEventCal(data){
	console.log(data);
}
(function($){
var $url = "index.php";

//PR functions
//$('#richtext_editor').trumbowyg();
	$('body').on('change', '#addSignature input', function(e) {
		//var $name = e.target.name;
		var $dad = $(this).closest(".contentarea_container");
    var $name = e.target.name;
		if($(this).is(":checked")){
		 // var $sigPath = path+"signatures."+$name+".html";

		  var $numSigs = $dad.find('.signature').length;
		  var $isOdd = ($numSigs % 2) == 1;
			var propsObject = {
				flag:"sdta-custom-insertSignature",
				oddEven:$isOdd,
				name:$name
			};
			makeAjaxCall(propsObject,function(data){
				if($isOdd){
					$dad.find('.eventcal tr:last-child .blockme:last-child').html(data);
				}else{
					$dad.find('.eventcal >tbody').append(data);	}
			});
		}else{
      console.log($dad);
      console.log($name);
				$dad.find("."+$name).remove();
		}
	});
var folderID;

    $('body').on('change', 'input[name="title[]"]', function() {
        //if tagged == yes
        var item = $(this);
        var content = item.val();
        console.log(content);
        item.parent().siblings().find('input[name="ad_content[]"]').val(
            content.toLowerCase().replace(/&|\u0027|\u2019|\u2018|and|,|:|\.|!|\u2013|\u2014|\u002d/g, "").replace(/san diego/g, "sd").replace(/[ ]/g, "_").replace(/__/g, "_")
						);
    });

    $('body').on('click', '.fa-italic', function() {
			var $dates2 = $(this).closest(".contentarea_container").find(".dates2");
			if($dates2.parent("em").length){
				$dates2.unwrap("</em>");
			}else{
				$dates2.wrap("<em>");
			}

		});
 		$('body').on('click', '.toggle', function(event) {
				var $main = $(this).closest(".contentarea_container");
				var $toggled = $main.hasClass("toggled");
				if($toggled == false){
					$(this).removeClass("fa-circle-o").addClass("fa-circle");
					$main.addClass("toggled");
					$main.find(".section_title").text("INDUSTRY TRENDS");
					$main.find("img").attr("src","http://image.updates.sandiego.org/lib/fe9e15707566017871/m/4/hotel-icon-modern.gif");
					$main.find(".insert").html('\
					Updated research reports are available <a href="http://www.sandiego.org/research">online</a> including:\
						<ul>\
							<li>5-year Travel Forecast</li>\
							<li>Visitor Profile Summary (Leisure & Business Overnight Travelers)</li>\
							<li>Visitor Industry Performance</li>\
						</ul>\
          Weekly Lodging Performance\
					');

				}else{
					$(this).removeClass("fa-circle").addClass("fa-circle-o");
					$main.find(".section_title").text("UPCOMING EVENTS");
					$main.removeClass("toggled");
					$main.find("img").attr("src","http://www.mylittleemailbuilder.com/images/MyLEB-placeholder_127x105.jpg");
					$main.find(".insert").html("");
				}
				console.log($toggled);

		});

    $('body').on('click', '.change-background', function(event) {
      var color = $(this).data("color");
      if($(this).attr("data-bg")){
        var bg = $(this).data("bg");
        console.log(bg);
      }else{
        var bg = "none";
      }
      $(this).closest(".contentarea_container").find(".fullpad").attr("bgcolor", "#"+color).attr('background', bg);
    });


  $('body').on('click', '.addEvent', function(event) {
      alert("this feature is not set up yet");
    return;
        var $dad = $(this).closest(".contentarea_container");
				var $numEvents = $dad.find('.event').length;
        var isOdd = ($numEvents % 2) == 1;

        if (isOdd) {
            $.get("layouts/events/lyt-singleEvent.html", function(data) {

                $dad.find('.eventcal tr:last-child .blockme:last-child').html(data);
            })
        } else {
            $.get("layouts/events/lyt-eventRow.html", function(data) {
                $dad.find('.eventcal >tbody').append(data);
            })
        }

    });

    $('body').on('click', '.removeEvent', function(event) {

        var $dad = $(this).closest(".contentarea_container");
				var $numEvents = $dad.find('.event').length;
        var isOdd = ($numEvents % 2) == 1;
        var $lastTD = $dad.find('.eventcal tr:last-child .blockme:last-child');
        console.log($lastTD.hasClass('right'));
        if ($lastTD.hasClass('empty')) {
            $lastTD.parent().remove();
        } else {

            $lastTD.empty().addClass('empty');
        }

    });
    $('body').on('click', '.addRow', function(event) {
       alert("this feature is not set up yet");
    return;
        var $dad = $(this).closest(".contentarea_container").find('.numbered-list-container');
        var $numRows = $dad.find('.numbered-list-item').length;
        console.log($dad);
        $.get("layouts/numbered-list/lyt-number-row.html", function(data) {
            $dad.find('.numbered-list-item').last().after(data);
            $dad.find('.number').last().text($numRows + 1);
        })


    });
    $('body').on('click', '.removeRow', function(event) {
        var $dad = $(this).closest(".contentarea_container");
        var $last = $dad.find('.list-item').last().remove();
    });


})(jQuery);
