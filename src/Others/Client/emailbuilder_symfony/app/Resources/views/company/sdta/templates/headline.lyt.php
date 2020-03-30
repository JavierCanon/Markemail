{% extends 'company/sdta/container.html' %}
{% block body %}
<table class="contentarea" width="100%" border="0" cellspacing="0" cellpadding="0">
<!--------------------------------- SECTION HEADLINE START ------------------------------------->
<!----------------------- dont forget to grab the opening table tag above ------------------------->
<!--------------------------------- SECTION HEADLINE START ------------------------------------->
	<tbody>
		<tr>
			<td class="fullpad" style="padding: 0 30px 0;" >
			<table width="100%" border="0" cellspacing="0" cellpadding="0">
				<tbody>
					<tr>
						<td style="padding-top: 20px;">
						

						<table class="headlinebar" width="100%" border="0" cellspacing="0" cellpadding="0">
							<tbody>
								<tr>
										
										<td valign="top" align="left" style="font-size: 18px; letter-spacing: 0.3px; color: #005f86;" class="subheadline inactive">
													
												<font style="font-family: 'Arial Narrow',Helvetica,sans-serif;"> 
												<!--[if (!mso 14)&(!mso 15)]><!--> 
												<font style="font-family: Oswald,'Arial Narrow',Helvetica,Arial,sans-serif;"> 
												<!--<![endif]--> 
												<span class="editable">SECTION TITLE</span>
												<!--[if (!mso 14)&(!mso 15)]><!--> 
												</font> 
												<!--<![endif]--></font></td>

									
										<td valign="top" align="right" style="font-size: 12px;" class="calltoaction"><a style="color: #88b6ca; font-family: Oswald,'Arial Narrow',Helvetica,Arial,sans-serif; text-decoration: none;" href="#">
										
										<font style="font-family: 'Arial Narrow', Helvetica, sans-serif;"> 
									<!--[if (!mso 14)&(!mso 15)]><!--> 
									<font style="font-family: Oswald, 'Arial Narrow', Helvetica, Arial, sans-serif;"> 
									<!--<![endif]-->
									<span class="editable sponsored">&nbsp;</span>
									          <!--[if (!mso 14)&(!mso 15)]><!--> 
									</font> 
									<!--<![endif]--></font>
									
										
										</a></td>
					
	
								</tr>
							</tbody>
						</table>
						</td>
					</tr>
					
					
					
				</tbody>
			</table>
			</td>
		</tr>
	</tbody>
<!--------------------------------- SECTION HEADLINE END ------------------------------------->
<!-------------------- dont forget to grab the closing table tag below ----------------------->
<!--------------------------------- SECTION HEADLINE END ------------------------------------->
</table>

{{ include('company/sdta/_controls.html', {controls:'headline',colors:true}) }}
{% endblock %}