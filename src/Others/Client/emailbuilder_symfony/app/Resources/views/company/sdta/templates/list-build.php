<?php
//$utm_campaign = $_REQUEST['utm_campaign'];
//$utm_source = $_REQUEST['utm_source'];
//$utm_medium = $_REQUEST['utm_medium'];

?>
{#
<form class="moduleform" title="tttdform" name="tttdform" action="{{ path('emailproject_make_event_layout') }}" method="post">
#}
<form class="moduleform" title="tttdform" name="tttdform" action="{{ route }}" method="post">







<div class="row">


{% if action is defined %}

<table width="100%" border="0" cellspacing="0" cellpadding="0" id="mainTable" class="simple-list inputTable">
  <tbody>   
    <tr>
      <th valign="top" bgcolor="#DADADA" class="rowlabel">#</th>
      <th valign="top" bgcolor="#DADADA" class="title">Title</th>
      <th valign="top" bgcolor="#DADADA" class="url">URL</th>
    </tr>
    {% for i in 0..16 %}
    <tr>
			<td class="rowlabel" width="30">{{ i }}</td>
      <td class="title"><input class="text" name="title[]" type="text" id="textfield" size="35" value="{% if listItems[i][0] is defined %}{{ listItems[i][0]|trim }}{% endif %}"></td>
      <td class="url"><input class="text" name="dest_url[]" type="text" id="textfield" size="75" value="{% if listItems[i][1] is defined %}{{ listItems[i][1]|trim }}{% endif %}"></td>
    </tr>
  {% endfor %}
       
  </tbody>
</table>

{% else %}

<table width="100%" border="0" cellspacing="0" cellpadding="0" id="mainTable" class="events inputTable">
  <tbody>   
    <tr>
      <th valign="top" bgcolor="#DADADA" class="rowlabel">#</th>
      <th valign="top" bgcolor="#DADADA" class="title">Event Title</th>
      <th valign="top" bgcolor="#DADADA" class="month">Event Line 1</th>
      <th valign="top" bgcolor="#DADADA" class="date">Event Line 2</th>
      <th valign="top" bgcolor="#DADADA" class="url" id="firstrow">Event URL</th>
      <th valign="top" bgcolor="#DADADA" class="content">Ad Content<br><span class="small">Enter 3-4 words max to identify your event in Google Analytics.</span></th>
    </tr>
    {% for i in 1..16 %}
    <tr>
			<td class="rowlabel">{{ i }}</td>
      <td class="title"><input class="text" name="title[]" type="text" id="textfield" size="35"></td>
      <td class="month"><input class="text" name="month[]" type="text" id="textfield" size="7"></td>
      <td class="date"><input class="text" name="date[]" type="text" id="textfield" size="7"></td>
      <td class="url"><input class="text" name="dest_url[]" type="text" id="textfield" size="75"></td>
      <td class="content"><input class="text" name="ad_content[]" type="text" id="textfield" size="25"></td>
    </tr>
  {% endfor %}
       
  </tbody>
</table>

{% endif %}


<input type="submit" value="Generate HTML" class="button generateHTML">
</div>

</form>
