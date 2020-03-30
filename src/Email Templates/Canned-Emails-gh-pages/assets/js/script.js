jQuery.extend (
  jQuery.expr[':'].containsCI = function (a, i, m) {
    var sText   = (a.textContent || a.innerText || "");
    var zRegExp = new RegExp (m[3], 'i');
    return zRegExp.test (sText);
  }
);
// as seen on http://stackoverflow.com/a/4936066

function filterList(header, list) {
  var form = $("<form>").attr({"class":"filterform","action":"#"}),
  input = $("<input>").attr({"class":"search searchTerm","type":"search", "itemprop":"query-input", "name":"searchbox", "placeholder": 'What are you looking for?'});
  $(form).append(input).appendTo(header);
  input.focus();
  var hash = window.location.hash.substring(1);
  if(hash) {
    $(input).val(hash);
    $matches = $(list).find('a:containsCI(' + hash + ')').parent();
    $('li', list).not($matches).slideUp();
    $matches.slideDown();
  }
  $(input)
  .change( function () {
    var filter = $(this).val();
    if(filter) {
      $matches = $(list).find('a:containsCI(' + filter + ')').parent();
      $('li', list).not($matches).slideUp();
      $matches.slideDown();
      window.location.hash = filter;
    } else {
      $(list).find("li").slideDown();
    }
    return false;
  })
  .keyup( function () {
    $(this).change();
  });
}

function applyclass() {
  var d = new Date();
  var n = d.getHours();
  if (n > 19)
    document.body.className = "night";
  else
    document.body.className = "day";
}

jQuery(function () {
  filterList(jQuery("#filter-form"), jQuery(".list-posts"));
  applyclass();
});
