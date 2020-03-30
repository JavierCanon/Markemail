{% extends 'company/sdta/container.html' %}
{% block body %}
<table class="contentarea" width="100%" border="0" cellspacing="0" cellpadding="0">
<!--------------------------------- OPEN CONTENT SECTION START ------------------------------------->
<!----------------------- dont forget to grab the opening table tag above ------------------------->
<!--------------------------------- OPEN CONTENT SECTION START ------------------------------------->
	<tbody>
		<tr>
			<td class="fullpad" style="padding: 15px 30px 15px;" >
			<table width="100%" border="0" cellspacing="0" cellpadding="0">
				<tbody>
					<tr>
						<td class="description" align="left" style="font-family: Helvetica, Arial, sans-serif; font-size: 14px; color: #434448; padding-top: 5px;">
						{% if rows is defined %}
						<ul class="simple-list-list">
						{% for row in rows %}
              <li style="font-family: Helvetica, Arial, sans-serif; font-size: 13px; line-height: 16px; color: #434448; padding-top: 5px;margin-bottom:5px;list-style-type:none;"><a style="text-decoration:none;" href="{{ row.url }}">{{ row.title }}</a></li>						
						{% endfor %}
            </ul>
						{% else %}

						<span class="editable textarea unedited">
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
            </span>
            {% endif %}
            
            
						</td>
					</tr>
				</tbody>
			</table>
			</td>
		</tr>
	</tbody>
<!--------------------------------- OPEN CONTENT SECTION END ------------------------------------->
<!----------------------- dont forget to grab the ending table tag below ------------------------->
<!--------------------------------- OPEN CONTENT SECTION END ------------------------------------->
</table>
{{ include('company/sdta/_controls.html', {controls:'paragraph',colors:true}) }}
{% endblock %}