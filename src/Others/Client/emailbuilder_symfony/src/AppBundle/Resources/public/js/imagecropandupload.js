function fillFields(e) {
  var file = e.target.result;
  console.log(e);
  var allTextLines = file.split(/\r\n|\n/);
  var lines = [];
  for (var i=0; i<allTextLines.length; i++) {
      var thisLine = allTextLines[i].split(',');
        $('input[name="' + thisLine[0] + '"]').val(thisLine[1]);
          var tarr = [];
          for (var j=0; j<thisLine.length; j++) {
              tarr.push(thisLine[j]);
          }
          lines.push(tarr);
  }
  console.log(lines);
}
function processData(csv) {
    var allTextLines = csv.split(/\r\n|\n/);
    var lines = [];
    for (var i=0; i<allTextLines.length; i++) {
        var data = allTextLines[i].split(';');
            var tarr = [];
            for (var j=0; j<data.length; j++) {
                tarr.push(data[j]);
            }
            lines.push(tarr);
    }
  console.log(lines);
}
function fadeOutCropbox(d,e){
  //console.log("my var is "+data['url']);
  $('#cropbox').fadeOut('slow', function() {
  //$('.activeImage').attr('src', 'http://image.updates.sandiego.org/lib/fe9e15707566017871/m/4/' + imageName);
  var $target = $(".activeImage");
  if($target.is(":input")){
    $target.val(d['url']);
  }else{
    $target.attr('src', d['url']);
  }

  $target.removeClass('activeImage');
  $(".activated").removeClass("activated");
  e.croppie('destroy');
  $('.fa-spinner').fadeOut();

  });
}
(function($){

//For image upload drop styling, should probably make tihs consistent
	  //with other styling, and add to CSS
	function cropAndUpload(e,t){
		console.log(t);

		//if($(this).is('.cloudinary')){$uploadLocation = 'cloudinary';}
		//$uploadLocation = 'myleb_folder';

		$(t).addClass('activeImage');
		var $w = $(t).data('width');
		var $h = $(t).data('height');
		var image = e.originalEvent.dataTransfer.files[0];
		imageName = new Date().getUTCMilliseconds()+ image.name;

		$('#cropbox').show();
		$uploadCrop = $('#cropbox').croppie({
			viewport: {
				width: $w,
				height: $h
			},
			boundary: {
				width: $w + 100,
				height: $h + 100
			}
		});
		$('#viewport_height').slider('value', $(".cr-viewport").outerHeight());
		$(".viewport_height_value").text($(".cr-viewport").outerHeight());

		var reader = new FileReader();

		reader.onload = function(e) {
			$uploadCrop.croppie('bind', {
				url: e.target.result
			});
		}
		reader.readAsDataURL(image);
    console.log(reader.result);
		$('.upload-cancel').on('click', function(ev) {
			$('#cropbox').fadeOut('fast');
			$uploadCrop.croppie('destroy');
			$(".activated").removeClass("activated");
			$(".activeImage").removeClass("activeImage");
		});

		$('.upload-result').on('click', function(ev) {
      if($(this).hasClass("no-crop")){
        uploadImage(reader.result);
      }else{
        $uploadCrop.croppie('result', {
						type: 'canvas',
						size: 'viewport',
						format: 'jpeg'
				}).then(uploadImage);
      }
		});
	}
  function uploadImage(resp) {
    var $uploadLocation = 'myleb_folder';
      $('.fa-spinner').fadeIn();
      $.ajax({
          url: "http://www.mylittleemailbuilder.com/processUploads2.php",
          type: "POST",
          data: {
              'file': resp,
              'imagename': imageName,
              'uploadLocation':$uploadLocation
          },
          dataType : 'json',
      }).done(function(data) {
          fadeOutCropbox(data,$uploadCrop);

      }).fail(function(data) {
          alert("there was a problem uploading your image");
          console.log(data);
      });
  }

	var $uploadCrop;
	var imageName;
	//var $uploadLocation = 'exacttarget';
	$(document).on({
		dragenter:function(e){
			e.preventDefault();
			$(this).addClass("activated");
		},
		dragleave:function(e){
			e.preventDefault();
			$(this).removeClass("activated");
		},
		dragover:function(e){
			e.preventDefault();
		},
		drop:function(e){
			e.preventDefault();
      switch($(this).data("type")){
        case "csv":
          var dropped = e.originalEvent.dataTransfer.files[0];
          var reader = new FileReader();
          reader.readAsText(dropped);
          reader.onload = fillFields;
          break;
        case "image":
        default:cropAndUpload(e,this);break;
      }

		},
	},".drop-area");




	$("#viewport_height").slider({
			range: "min",
			orientation: "vertical",
			min: 0,
			max: 450,
			step: 5,
			slide: function(event, ui) {
					//$( "#amount" ).val( ui.value );

				$('.cr-viewport').css({
						'height': ui.value
				});
				$('.viewport_height_value').text(ui.value);

			}
	});




})(jQuery)
