{% extends 'company/sdta/container.html' %}
{% block body %}
<table class="contentarea" border="0" width="100%" cellspacing="0" cellpadding="0">
	<tbody>
		<tr>
			<td class="fullpad" style="padding: 0px 30px 40px;">
				<table width="100%" border="0" cellspacing="0" cellpadding="0">
					<tbody>
						<tr>
							<td style="padding-top: 15px;">
							
							
							
							
							
							
							
							
								<table class="headlinebar" width="100%" border="0" cellspacing="0" cellpadding="0">
									<tbody>
										<tr>
											<td class="subheadline" align="left" valign="top" style="font-size: 18px; color: #005f86; border-top-width: 1px; border-top-style: solid; border-top-color: #dfe0e0; padding-top: 15px;">
												<font style="font-family: 'Arial Narrow', Helvetica, sans-serif;">
													<!--[if (!mso 14)&(!mso 15)]><!-->
													<font style="font-family: Oswald, 'Arial Narrow', Helvetica, Arial, sans-serif;">
														<!--<![endif]-->
														<span class="editable section_title">UPCOMING EVENTS</span>
														<!--[if (!mso 14)&(!mso 15)]><!-->
													</font>
													<!--<![endif]-->
												</font>
											</td>
										</tr>
									</tbody>
								</table>
								
								
								
								
								
								
								
							</td>
						</tr>
						<tr>
							<td style="padding-top: 30px;">
								<div class="insert description" align="left" style="font-family: Helvetica, Arial, sans-serif; font-size: 12px; color: #434448; padding-top: 5px;">

								</div>


								<table width="100%" border="0" cellspacing="0" cellpadding="0">
									<tbody>
										<tr>
											<td class="blockme" style="padding-right: 5px;" width="50%" valign="top" align="left">
												<table width="100%" border="0" cellspacing="0" cellpadding="0">
													<tbody>
														<tr>
															<td width="50%" style="padding-right: 5px;" valign="top" class="droppable one">


																{{ include('company/sdta/templates/four-up-image.part.html') }}


															</td>
															<td width="50%" style="padding-left: 5px;" valign="top" class="droppable two">


																{{ include('company/sdta/templates/four-up-image.part.html') }}






															</td>
														</tr>
													</tbody>
												</table>
											</td>
											<td class="blockme" style="padding-left: 5px;" width="50%" valign="top" align="left">
												<table width="100%" border="0" cellspacing="0" cellpadding="0">
													<tbody>
														<tr>
															<td width="50%" style="padding-right: 5px;" valign="top" class="droppable three">



																{{ include('company/sdta/templates/four-up-image.part.html') }}



															</td>
															<td width="50%" style="padding-left: 5px;" valign="top" class="droppable four">




																{{ include('company/sdta/templates/four-up-image.part.html') }}



															</td>
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
				</table>
			</td>
		</tr>
	</tbody>
</table>

{{ include('company/sdta/_controls.html', {controls:'four-up',colors:true}) }}
{% endblock %}