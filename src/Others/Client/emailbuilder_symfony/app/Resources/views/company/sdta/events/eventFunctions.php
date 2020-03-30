<?php
function createEvent($month,$date,$dest_url,$title){
	
	return '
	  <table cellspacing="0" cellpadding="0" border="0" class="event draggable">
										<tbody>
											<tr>
												<td width="60" align="center" valign="top">
												<i class="fa fa-link link left fa-1x" aria-hidden="true"></i>
												<table width="100%" border="0" cellspacing="0" cellpadding="0">
													<tbody>
														<tr>
															<td width="60" bgcolor="#ee7421" align="center" style="font-size: 15px; letter-spacing: 0.5px; line-height: 20px; color: #ffffff; padding: 10px 0px; width: 60px;">		
															<div class="eventDatepicker fa fa-calendar"></div>								
															<font style="font-family: \'Arial Narrow\', Helvetica, sans-serif;"> 
															<!--[if (!mso 14)&(!mso 15)]><!--> 
															<font style="font-family: Oswald, \'Arial Narrow\', Helvetica, Arial, sans-serif;"> 
															<!--<![endif]--><span class="date editable textarea">'.$month.'<br />
															'.$date.'</span><!--[if (!mso 14)&(!mso 15)]><!--> 
															</font> 
														  <!--<![endif]--></font>      </td>
														</tr>
													</tbody>
												</table>
												</td>
												<td style="padding-left: 15px; padding-right: 7px;">
												<table width="100%" cellspacing="0" cellpadding="0" border="0">
													<tbody>
														<tr>
															<td align="left" class="title"><a style="font-size: 16px; letter-spacing: 0.3px; line-height: 22px; color: #434448; text-decoration: none;" href="'.$dest_url.'">
															<font style="font-family: \'Arial Narrow\', Helvetica, sans-serif;"> 
															<!--[if (!mso 14)&(!mso 15)]><!--> 
															<font style="font-family: Oswald, \'Arial Narrow\', Helvetica, Arial, sans-serif;"> 
															<!--<![endif]--> <span class="editable textarea eventname">'.$title.'</span><!--[if (!mso 14)&(!mso 15)]><!--> 
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
											
	';	
};



function leftEvent($month,$date,$dest_url,$title,$i){
return '
<!-- START: ROW -->
<tr>
<td width="50%" valign="top" align="left" style="padding-right: 5px; padding-top: 20px; padding-bottom: 20px; border-bottom-width: 1px; border-bottom-style: solid; border-bottom-color: #dec79e;" class="blockme left">'.   
		 createEvent(
		strtoupper($month),
		strtoupper($date),
		$dest_url,
		strtoupper(stripslashes($title))).'</td>';
		
		
		
		
		

}
function rightEvent($month,$date,$dest_url,$title,$i){
	
	return '
	
	
	 <td width="50%" valign="top" align="left" style="padding-left: 5px; padding-top: 20px; padding-bottom: 20px; border-bottom-width: 1px; border-bottom-style: solid; border-bottom-color: #dec79e;" class="blockme right">'.
		
		createEvent(
		strtoupper($month),
		strtoupper($date),
		$dest_url,
		strtoupper(stripslashes($title))).'</td></tr><!-- END: ROW -->';

}



?>