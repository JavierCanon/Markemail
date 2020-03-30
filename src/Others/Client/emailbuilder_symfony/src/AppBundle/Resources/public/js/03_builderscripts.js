function closeLoader(){
	$('#load').dialog('close');
	$('#loadContent').empty();
}
function pad(n) {
    return (n < 10) ? ("0" + n) : n;
}
function getQueryVariable(variable) {
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        if (pair[0] == variable) {
            return pair[1];
        }
    }
    return (false);
}
function getProjectID(){
  var url = window.location.href;
  //var match = url.match(/emailproject\/(\d+)\/build/);
  var pid = url.split("/");
  console.log(pid[4]);
  return pid[4];
  /*
  if (match) {
    return match[0];
  }else{
    return 1;
  }
  */
}
function deactivate(){
	$('.activated').removeClass('activated');
}
function makeAjaxCall(options,baseURL,callback){
	$.ajax({
		type: "POST",
		dataType: "text",
		url: baseURL,
		data: {
			options:options,
			isAjax: true
		}
	}).done(function (data) {
			callback(data);
	});
}
//Why did I name this codePop?
function codePop(code,addButton){
  var $button = '';
  if(addButton==true){
    var $button = '<button class="savecode">Save Code Changes</button>';
  }
  $('#loadContent').html('<textarea id="rawhtml" cols="130" rows="30" style="font-family:monospace">' + code + '</textarea>'+$button);
  $('.ui-dialog').addClass('grabcode');
  $('#load').dialog('open');
}
 $('body').on('click', '.events-build, .list-build', function(event) {
   var $dad = $(this).closest(".contentarea_container");
   $dad.addClass("use-event-build-list");

   var $module = $(this).data("module");
   var $route = $(this).data("route");
   var $flag = $(this).data("action");
   var propsObject = {
    flag:$flag,
    module:$module,
    route:$route
  };

   if($dad.hasClass("simple-list")){
     var $listItems = [];
     $($dad.find("ul li")).each(function(i,li){
      $listItems.push(
        new Array($(li).text(),$(li).find("a").attr("href"))
      );
     });
     propsObject.listItems = $listItems;
    }

  makeAjaxCall(propsObject,$route,function(data){
    $('#loadContent').html(data);
    $('#load').dialog('open');
  });
 });

$('body').on('submit','.moduleform', function(e) {
    e.preventDefault();
    formData = $(this).serialize();
    var $action = $(this).attr('action');

    makeAjaxCall(formData,$action,function(data){
      $(".use-event-build-list").replaceWith(data);
      closeLoader();
    });
});

insertDroppedModule=function(html,module){
	if($(".empty")[0]){
			$('#container').html(html);
		}else{
			$(".droppedHere").replaceWith(html);
			//$(".droppedHere").after(html);
		}
		//$(".dropzone, .ui-droppable").remove();
		//$(".droppedHere").removeClass("droppedHere");
		deactivate();
		//$('.ui-droppable').droppable("destroy");
}

//maybe there's a better way to do this, I need to call this function
//every.single.time an event happens because even after a drag N drop
//jquery doesn't see the new draggable items without initializing them again
function doDraggable() {
    $('.numbered-list').find('.numbered-list-container').sortable({
        handle: ".drag",
        stop: function(event, ui) {
            var $listItem = $(this).find('.numbered-list-item');
            $.each($listItem, function(index) {
                $(this).find('.number').text(index + 1);
            });
        }
    });
	  $(".fullhtml").click(function(e){
      e.preventDefault();
    });
    $(".fullhtml").draggable({
			scroll:true,
			helper:"clone",
    	revert: function (event, ui) {
    		$('.draggedFrom').removeClass('draggedFrom');
        $(".dropzone").remove();
    		return true;
    	},
    	start: function (event, ui) {
    		$container = $("#container");
				var $contentblock = $(".contentarea_container");
				$.each($contentblock, function(){
					$(this).after('<div class="dropzone"><div class="dropzone--target"></div></div>');
					//$(this).before('<div class="dropzone" style="height:20px;"></div>');
				});

    		$('.empty, .dropzone').droppable({
    			hoverClass: "activated",
          tolerance: "touch",
    			drop: function (event, ui) {
						$(this).addClass("droppedHere").removeClass("dropzone");
						var propsObject = {
							flag:"dropModule",
							module:ui.draggable.data("module")
						};
						makeAjaxCall(propsObject,"/emailproject/ajax",insertDroppedModule);
    				doDraggable();

    			}
    		});

    	}
    });

    $(".draggable").draggable({
        revert: function(event, ui) {
          $('.draggedFrom').removeClass('draggedFrom');
          $('.dropzone').remove();
          return true;
        },
        start: function(event, ui) {
          var $p = $(this).parent();
          $p.addClass('draggedFrom');
          var $html = $p.html();

          //$p.droppable('disable');

          $('.blockme, .nopad').not($p).not(".contentarea_container").droppable({
              hoverClass: "activated",
              drop: function(event, ui) {
								if(ui.draggable.find('.nopad').hasClass("padLeft")){
									ui.draggable.find('.nopad').css({'padding-right':'15px','padding-left':'0'}).addClass('padRight').removeClass('padLeft');
								}else if(ui.draggable.find('.nopad').hasClass("padRight")){
									ui.draggable.find('.nopad').css({'padding-left':'15px','padding-right':'0'}).addClass('padLeft');
								}

                  var $html = $(this).html();
                  $(this).empty();
                  //ui.draggable.detach().appendTo($(this)).removeAttr('style');
                  ui.draggable.detach().appendTo($(this)).css({
                      top: 0,
                      left: 0,
                      position: 'relative'
                  });
                  $('.draggedFrom').html($html).removeClass('draggedFrom');
                  $('.ui-droppable').droppable("destroy");
                  //$(this).removeClass('activated');
                  doDraggable();
                }
            });
            //console.log($html);
        }




    });
}
$(document).ajaxComplete(function() {
    doDraggable();
});
$(document).ready(function() {
  doDraggable();
	var $container = $("#container");
/////////////////////////////
/////////DO SOMETHING WITH THIS, IT'S AN AJAX CALL I MISSED
/////LOOKS LIKE IT CHECKS TO SEE IF THE CONTAINER HAS THE NEW PROJECT CLASS
////// THEN DOES SOME AJAXY STUFFY, PROBABLY NOT A GOOD WAY TO DO THIS, SHOULD HANDLE IN INDEX.PHP	///////////////
	if($container.hasClass("new_project")){
		var $tmp = $container.data("template");
		$.get("templates/"+$tmp+".html", function(data) {
			$container.append(data);
			$container.removeClass("new_project");
			$(".senddate").text(fullMonthName + " " + nextFriday + ", " + year);
		});
	}
////////////////////////////////////////////////////////////////////////
		$("#project_list").tablesorter();

  //image uploader stuff used to be here

    var w = $(window).width() - 200;
    var h = $(window).height() - 100;
    $('#load').dialog({
        resizable: false,
        height: h,
        width: w,
        dialogClass: 'noTitle',
        modal: true,
        autoOpen: false,
        resizable: true,
        position: {
            my: "left top",
            at: "left+20 top+20",
            of: window
        },
        close: function(event, ui) {
            $('#loadContent').empty();
            //$(this).dialog('destroy');
        }
    });


	  $('#clearContainer').click(function() {
      if(window.confirm("Are you sure?")){
      	$('#container').html("<span class=\"empty\"></span>");
      }
	  });
	  $('body').on('click', '.grabcode .ui-icon-closethick, .savecode', function() {
      $('.wrapped').removeClass("wrapped").find('.contentarea').unwrap();
      $('.ui-dialog').removeClass('grabcode');
	  });

	 $('body').on('click', '.savecode', function(event) {
		 var $newCode = $("#rawhtml").val();
     $(".replaceMe").replaceWith($newCode);
		 closeLoader();
	 });

	$("body").on("click","#grabAllCode, .grab-module-code, .edit-module-code, #syncToET, #saveProject",function() {
		if($("#container").find(".unedited").length){
	    if(!window.confirm("Whoa, it looks like you have some unfilled content areas")){
	      return;
	    }
		}
		$(".wrapped").removeClass("wrapped");
		var theCode = '';
		var id = $(this).attr("id");
	  var action = $(this).data("action");
	  console.log(action);

		switch(action){
			case "syncToET":
				$('.contentarea_container').each(function() {
	        theCode += $(this).find('.contentarea').wrap('<p/>').parent().html();
	    	});

				var propsObject = {
					flag:"syncToExacttarget",
					html:theCode,
					etID:$("#exacttarget_id").val()
				};
				makeAjaxCall(propsObject,"/emailproject/ajax",function(data){
					console.log(data);//need to make this update a DIV onscreen
					return;
				});
				break;
			case "grabAllCode":
				$('.contentarea_container').each(function() {
						$(this).addClass("wrapped");
						theCode += $(this).find('.contentarea').wrap('<p/>').parent().html();
				});
				break;
			case "saveProject":
				var saveObject = {
					flag:"saveProject",
					html : $('#container').clone().find('.draggable').removeClass('ui-draggable-handle ui-draggable').end().html(),
					projectNum : getProjectID(),
					tag_4_ga: $("#tag4ga").is(":checked"),
					template: $("#template").val(),
					utm_source: $('#utm_source').val(),
					utm_campaign: $('#utm_campaign').val(),
					utm_medium: $('#utm_medium').val()
				};
				makeAjaxCall(saveObject,"/emailproject/ajax",function(data){
	        $('.filled').each().unwrap();
	        $(".saved").fadeIn().delay(3000).fadeOut();
				});
	      break;
			default:
				$(this).closest(".contentarea_container").addClass("wrapped");
				theCode = $(this).closest(".contentarea_container").find('.contentarea').wrap('<p/>').parent().html();
				break;
		}
	  if(action=="grabModuleCode" || action == "grabAllCode"){
	    codePop(theCode,false);
	  }else if(action=="editModuleCode"){
	    $(".wrapped").find(".contentarea").addClass("replaceMe");
	    codePop(theCode,true);
	  }
	});
  //$('#saveproject').click(function() {   //used to have save functions here, merged with above });

//Module control functions start
    $("body").on("click", ".remove", function(e) {
      e.preventDefault();
			var $me = $(this);
			if($me.hasClass("self")){
				$me.parent().remove();
			}else{
				$me.closest(".contentarea_container").remove();
				if($("#container").find(".contentarea_container").length < 1){
					$("#container").html("<span class=\"empty\"></span>");
				}
			}
    });

    $('#container').sortable({
        handle: ".drag"
    });

    $("body").on("click", ".duplicate", function() {
			var $dad = $(this).closest(".contentarea_container");
			var $moduleHTML = $dad.wrap("</p>").parent().html();
			$dad.addClass("duplicated");
			$('#container').append($moduleHTML);
			$(".duplicated").unwrap().removeClass("duplicated");
		});

 		$('body').on('click', '.toggle_sponsored', function(event) {
			var $me = $(this).closest(".contentarea_container").find(".sponsored");
			if($me.hasClass("on")){
				$me.text(" ");
				$me.removeClass("on");
			}else{
				$me.text("SPONSORED");
				$me.addClass("on");
			}
		});
 		$('body').on('click', '.remove_headline', function(event) {
			var $me = $(this);
			$me.closest(".contentarea_container").find(".headlinebar").remove();
			$me.siblings(".toggle_sponsored").remove();
		});

		$(".fa-calendar").click(function() {
       $(".eventDatepicker").show();
    });
//Module control functions end

//Control panel functions start
    $('#minimize_control_panel').click(function() {
      var $cPanel = $(this).parent();
      if ($cPanel.hasClass('open')) {
        $cPanel.removeClass('open').addClass('minimized');
        $(this).find('.fa').removeClass('fa-chevron-down').addClass('fa-chevron-up');
      } else {
        $cPanel.removeClass('minimized').addClass('open');
        $(this).find('.fa').removeClass('fa-chevron-up').addClass('fa-chevron-down');
      }
    });

		$('body').on('click', '.fa-sliders', function(e) {
			var $dad = $(this).parent();
			makeAjaxCall({path:"emailproject/sliders.html"},"/emailproject/get_html",function(data){
				$dad.prepend(data);
				$(".control-overlay").fadeIn();

				$(".padding-slider").slider({
		        range: "min",
		        min: 0,
		        max: 100,
		        step: 10,
		        slide: function(event, ui) {
	            //$( "#amount" ).val( ui.value );
							var dir = "padding-" + $(this).data("direction");
              $dad.closest(".contentarea_container").find(".fullpad").css( dir, ui.value );
              $('#'+dir).find('.value').text(ui.value);
		        }
		    });










			});

		});


    $("#resize_control_panel").resizable({ //on resize, check cpanel width and change image size
        handles: "w"
    });
    $(".resize_control_panel").resizable({ //on resize, check cpanel width and change image size
        handles: "w"
    });
   //control panel functions end





//Text edition functions start
    $('body').on('click', '.editable', function(event) {
        event.preventDefault();
        var $me = $(this);

        var $fontsize = $me.closest(".dest_url, .description").css("font-size");

        var $content = $me.html();

        if (!$me.hasClass("editing")) {
            if ($me.hasClass('textarea')) {
                $me.wrap("<textarea autofocus class=\"editing\" style=\"font-size:"+$fontsize+"\"/>");

            } else {
                $me.wrap("<input autofocus class=\"editing\" style=\"font-size:"+$fontsize+"\"/>");
            }
            $me.closest(".editing").val($content);
            $(".editing").before('\
                <div class="fontSize display"></div>\
                <i class="fa fa-plus fontSize" aria-hidden="true" title="Increase font size"></i>\
                <i class="fa fa-minus fontSize" aria-hidden="true" title="Decrease font size"></i>');

        }
    });

    $("body").on("click", ".fontSize", function(event) {
      event.preventDefault();
      var $me = $(this);
      $currentFontSize = $me.closest("td").css("font-size");
      $currentFontSize = Number($currentFontSize.replace("px",""));

      $currentLineHeight = Number($me.closest("td").css("line-height").replace("px",""));
      console.log($currentLineHeight);
        if($me.hasClass("fa-plus")){
          $currentFontSize++;
          $currentLineHeight++;
        }else{
          $currentFontSize--;
          $currentLineHeight--;
        }
      $me.closest("td").css("font-size",$currentFontSize+"px");
      $me.closest("td").css("line-height",$currentLineHeight+"px");
      $(".editing").css("font-size",$currentFontSize+"px");
      $(".fontSize.display").text($currentFontSize+"px");
    });
    $('body').on('click', '.editing', function(event) {
        event.preventDefault();
    });

			//next function pops up box where you input img or href link
    $('body').on('click', '.fa-external-link, .link', function(e) {
			e.preventDefault();
			var $me = $(this);

			if($me.hasClass("link")){
				var $link = $(this).closest("a").attr("href");
				var $type = $me.data("linktype");
        var $pos = $me.data("position");
				$(this).closest('.blockme, .first_buffer_row, .pasted, .linkinput').prepend('<input autofocus class="editing link-input" data-position="'+$pos+'"/>');
				$('.editing').val($link).focus();
			}else{
				$me.addClass("active_editing");
				$me.closest(".blockme").prepend('<input autofocus class="editing img-input"/>');
			}
    });

    $('body').on('keydown', '.editing', function(e) {


        if (e.which == 9) { //9 is tab key, 13 is enter key
						var $me = $(this);
          $(".fontSize").remove();
            //var $type = $me.data('linktype');
						var $newValue = $me.val();
            var $dad = $me.closest('.blockme, .numbered-list-item, .pasted');
				    var $container = $me.closest(".contentarea_container");


            if ($me.hasClass('link-input')) {
              var $pos = $($me).data("position");

              if($pos != "undefined"){
                $dad.find("."+$pos + " a").attr('href', $newValue);
              }else{
                $dad.find('a').attr('href', $newValue);
              }
              $container.prepend("<div class=\"linked_url\">Module linked to URL</div>");
              //$('.linking').addClass('linked').removeClass('linking');
              $me.remove();

            } else if ($me.hasClass('img-input')) {
              var $updateMe = $(".active_editing");
              $updateMe.attr("src",$newValue);
              $updateMe.removeClass("active_editing");
                $me.remove();
						} else {
                if ($me.find('span').is('.maintitle, .date, .eventname, .dates, .section_title, .dates2')) {
                    $newValue = $newValue.toUpperCase();
                }
                $me.find('span').removeClass('unedited').html($newValue.replace(/(?:\r\n|\r|\n)/g, '<br />')).unwrap();
                return false;
            }


        }

    });
//Text editor functions end




		$('body').delegate('.eventDatepicker', 'click', function(event) {
			$(this).parent().find(".date").addClass("active");
			$(this).datepicker(
			"dialog",
			"",
			function(v){
				//var cleanDate = v.split("/");
				//v = v.replace(/\//g, "\n");
				$(".active").html(v.toUpperCase()).removeClass("active");
			},
			{
				minDate: -1,
				showButtonPanel: true,
				dateFormat: "M<br>dd"
			}
			);
			//$(this).datepicker( "dialog", "10/12/2012" );
    });
var $r1, m1, d1, date1;
     $('body').delegate('.eventDatepickerRange', 'click', function(event) {
			 event.preventDefault();
				$(this).parent().find(".date").addClass("active");
				$(this).datepicker(
				{
					minDate: -1,
					showButtonPanel: true,
					onSelect:function(v){
						//console.log(m);
						var dPickerDate = $(this).datepicker( "getDate" );
						if($r1 == undefined){
							var dTime = d.getTime();
							m1 = monthNames[dPickerDate.getMonth()];
							d1 = dPickerDate.getDate();
							var vTime = dPickerDate.getTime();
							console.log(vTime<dTime);
							if(vTime<dTime){
								date1 = "THRU";
							}else{
								date1 = m1.toUpperCase() + " " + d1;
							}

							$r1 = v;

						}else{

							var m2 = monthNames[dPickerDate.getMonth()];
							var d2 = dPickerDate.getDate();
							if(m1 != m2 || date1 == "THRU"){
								//different months

								var string = date1 + "<br>-" + m2.toUpperCase() + " " + d2;
								$(".active").html(  string ).removeClass("active");
							//	$(this).find(".ui-datepicker").hide();
							}else{
								//same month
								var string = m2.toUpperCase();
								string+= "<br>" + d1 + "-" + d2;
								$(".active").html(  string ).removeClass("active");
								//$(this).hide();
							}

						$(".eventDatepickerRange").datepicker("destroy");
						$r1 = undefined;

						}
						console.log(v);
					}
				}
				);
				//$(this).datepicker( "dialog", "10/12/2012" );
    });

//SCREENSHOT STUFF




//SHAREABLE STUFF BELOW
    $('#twitter_text').on('keyup', function() {
        var charsLeft = 120 - $(this).val().length;
        $('#charcount').text(charsLeft);
        if (charsLeft < 10) {
            $('#charcount').css({
                'color': 'red'
            });
        }
    });
    $("#datepicker").datepicker();


    $('#slider').slider({
        min: 0,
        max: 1440,
        step: 15,
        slide: function(event, ui) {
            var hours = Math.floor(ui.value / 60);
            var minutes = ui.value - (hours * 60);

            if (hours.toString().length == 1) hours = '0' + hours;
            if (minutes.toString().length == 1) minutes = '0' + minutes


            $('#time').val(hours + ':' + minutes);

        }

    });



});
