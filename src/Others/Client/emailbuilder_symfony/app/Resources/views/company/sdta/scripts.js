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
var startDate = getNearestTuesday(todaysDate, todaysWeekday);
var nextFriday = getNearestFriday(todaysDate,todaysWeekday);
var endDate = Number(startDate + 5);
var yearMonDay = year+"-"+(monthNums[d.getMonth()])+"-"+todaysDate;
var monDayYear = fullMonthName + " " + todaysDate + ", " + year;
function getNearestTuesday(t, wd) {
    var sDate; //this will be set to Tuesday below.
    switch (wd) {
        case 0:
            sDate = t + 2;
            break;
        case 1:
            sDate = t + 1;
            break;
        case 2:
            sDate = t;
            break;
        case 3:
            sDate = t - 1;
            break;
        case 4:
            sDate = t - 2;
            break;
        case 5:
            sDate = t - 3;
            break;
        case 6:
            sDate = t - 4;
            break;
        default:
            Date = t;
    }
    return sDate;
}
function getNearestFriday(t, wd) {
    var sDate; //this will be set to Friday below.
    switch (wd) {
        case 0:
            sDate = t + 5;
            break;
        case 1:
            sDate = t + 4;
            break;
        case 2:
            sDate = t+3;
            break;
        case 3:
            sDate = t + 2;
            break;
        case 4:
            sDate = t + 1;
            break;
        case 5:
            sDate = t;
            break;
        case 6:
            sDate = t - 1;
            break;
        default:
            Date = t;
    }
    return sDate;
}
function tagForGA(url, tmp) {
    var today = new Date();
    if (tmp == "tttd") {
        var cname = "Top Things to Do";
        var source = $('#utm_source').val();//year + '_' + monthAbbr + '_' + startDate + '-' + endDate;
    } else if (tmp == "monthly") {
        var cname = "Consumer Newsletter Monthly";
        var source = monthNames[d.getMonth() + 1];
    }
    var taggedURL = url + '?utm_campaign=' + cname + '&utm_source=' + source + '&utm_medium=email';
    return taggedURL;
}
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
		if($(this).is(":checked")){		
		 // var $sigPath = path+"signatures."+$name+".html";
		  
		  var $numSigs = $dad.find('.signature').length;
		  //var isOdd = ($numSigs % 2) == 1;	
			
			
			var propsObject = {
				flag:"sdta-custom-insertSignature",
				oddEven:($numSigs % 2) == 1,
				name:e.target.name
			};
				
			//makeAjaxCall(propsObject,updateEventCal);
			makeAjaxCall(propsObject,function(data){
				if(isOdd){
					$dad.find('.eventcal tr:last-child .blockme:last-child').html(data);
				}else{
					$dad.find('.eventcal >tbody').append(data);	}			
			});
		/*	
		  $.ajax({
				type: "POST",
				dataType: "text",
				url: $url,
				data: {
					oddEven: isOdd,
					name:$name,
					isAjax: true
				}
			}).done(function (data) {
				if(isOdd){
					$dad.find('.eventcal tr:last-child .blockme:last-child').html(data);
				}else{
					$dad.find('.eventcal >tbody').append(data);
				}
			});
		  
		  //code before was commented out until just now 1-13-2017 4:10pm
		  	
		  if (isOdd) {
			  $.get($sigPath, function(data) {
					  $dad.find('.eventcal tr:last-child .blockme:last-child').html(data);
			  })
		  } else {
			  $.get(path+"signatures.row.html", function(data) {
				  $dad.find('.eventcal >tbody').append(data);
				  $.get($sigPath, function(data) {
						  $dad.find('.eventcal tr:last-child .blockme:first-child').html(data);
				  });					
					  
			  })
		  }	
		  */	
		}else{
				$dad.find("."+$name).remove();
		}

		
		
		//var $nextEmpty = $(this).closest("contentarea_container").find(".empty");
		
	});
var folderID;		
	$("#template").change(function(e){
			var $val = $(this).val()	;
			switch($val){
				case "Press Release": 
					$("#project_name").val(yearMonDay+" - Press Release - ");
					$("#et_folder_input").val(40560);
					$("#template").val("pressrelease");
					break;				
				case "execReport": 
					$("#project_name").val(year +"-"+monthNum+"-"+pad(nextFriday)+"-Executive-Update-Email");
					$("#subject").val("SDTA Board of Directors Update - "+fullMonthName+" "+nextFriday+", "+year);
					$("#et_folder_input").val(343713);
					break;
				case "Top Things to Do":
					$("#project_name").val(yearMonDay+" TTTD");
					$("#subject").val("San Diego's Top Things to Do This Weekend");			
					$("#et_folder_input").val(324989);
					$("#template").val("tttd");	
					break;
				case "Consumer News":
					$("#project_name").val(year + " " + nextMonthAbbr.toUpperCase() + " Consumer News");
					$("#subject").val("Next Month in San Diego");
					$("#et_folder_input").val(40559);
					$("#template").val("monthly");			
					break;							
				default:break;
			}
		});		


    $('body').on('change', '#template', function() {
        var id = this.value;
        if (id == "tttd") {
            var cname = "Top Things to Do";
            var source = year + "_" + monthAbbr + "_" + startDate + "-" + endDate;
        } else {
            var cname = "Consumer Monthly";
            var source = nextMonthAbbr;

        }
        $("#utm_campaign").val(cname);
        $("#utm_source").val(source);

    });

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

    $('body').on('click', '.ltblue', function(event) {
        $(this).closest(".contentarea_container").find('.fullpad').attr('bgcolor', '#dbe7ef').attr('background', 'none');

    });
    $('body').on('click', '.white', function(event) {
        $(this).closest(".contentarea_container").find('.fullpad').attr('bgcolor', '#ffffff').attr('background', 'none');

    });
    $('body').on('click', '.orange', function(event) {
        $(this).closest(".contentarea_container").find('.fullpad').attr('bgcolor', '#fddea6').attr('background', 'http://image.exct.net/lib/fe6e15707166047a7715/m/1/sdta_nl_small_texture_tan.jpg');

    });


  $('body').on('click', '.addEvent', function(event) {
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