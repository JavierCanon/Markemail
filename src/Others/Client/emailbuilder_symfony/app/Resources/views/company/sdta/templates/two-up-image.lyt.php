{% extends 'company/sdta/container.html' %}
{% block body %}
<table class="contentarea twocol" width="100%" border="0" cellspacing="0" cellpadding="0">
	<tbody>
		<tr>
			<td class="fullpad bgcolor toppad" style="padding:15px 30px 30px;">
			<table width="100%" border="0" cellspacing="0" cellpadding="0">
				<tbody>
					<tr>
						<td class="blockme droppable" style="padding-right: 5px;" width="50%" valign="top" align="left">
            
            {{ include('company/sdta/templates/two-up-image.part.html', {class:'left'}) }}
            
            
						

						</td><td class="blockme droppable" style="padding-left: 5px;" width="50%" valign="top" align="left">



{{ include('company/sdta/templates/two-up-image.part.html', {class:'right'}) }}





					</td>
					</tr>
				</tbody>
			</table>
			</td>
		</tr>
	</tbody>
</table>
{{ include('company/sdta/_controls.html', {controls:'two-up',colors:true}) }}
{% endblock %}