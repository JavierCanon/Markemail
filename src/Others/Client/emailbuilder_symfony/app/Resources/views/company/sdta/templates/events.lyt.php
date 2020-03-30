{% extends 'company/sdta/container.html' %}
{% block body %}
<table class="contentarea" width="100%" cellspacing="0" cellpadding="0" border="0">
  <tbody>
    <tr>
      <td style="padding-top: 0px;"><table width="100%" cellspacing="0" cellpadding="0" border="0">
          <tbody>
            <tr>
              <td bgcolor="#fddea6" background="http://image.exct.net/lib/fe6e15707166047a7715/m/1/sdta_nl_small_texture_tan.jpg" style="padding: 25px 30px 15px;" class="fullpad"><table width="100%" cellspacing="0" cellpadding="0" border="0" class="headlinebar">
                  <tbody>
                    <tr>
                      <td valign="top" align="left" style="font-size: 18px; color: #005f86;" class="section_title"><font style="font-family: 'Arial Narrow', Helvetica, sans-serif;"> 
                        <!--[if (!mso 14)&(!mso 15)]><!--> 
                        <font style="font-family: Oswald, 'Arial Narrow', Helvetica, Arial, sans-serif;"> 
                        <!--<![endif]--> 
                        <span class="editable">UPCOMING EVENTS</span> 
                        <!--[if (!mso 14)&(!mso 15)]><!--> 
                        </font> 
                        <!--<![endif]--></font></td>
                      <td valign="top" align="right" style="font-size: 12px;" class="calltoaction top-right"><a style="text-decoration: none; color: #979597;" href="#"> <font style="font-family: 'Arial Narrow', Helvetica, sans-serif;"> 
                        <!--[if (!mso 14)&(!mso 15)]><!--> 
                        <font style="font-family: Oswald, 'Arial Narrow', Helvetica, Arial, sans-serif;"> 
                        <!--<![endif]--> 
                        <span class="editable">SEE ALL TOP THINGS TO DO</span> 
                        <!--[if (!mso 14)&(!mso 15)]><!--> 
                        </font> 
                        <!--<![endif]--></font> </a></td>
                    </tr>
                  </tbody>
                </table>
                <table width="100%" cellspacing="0" cellpadding="0" border="0" class="eventcal">
                  <tbody>
                    <!-- START: ROW -->
                    {% if rows is defined %}
                      {% for row in rows %}
                    {% if loop.index0 % 2 == 0 %}
                    <tr>
                      <td width="50%" valign="top" align="left" style="padding-right: 5px; padding-top: 20px; padding-bottom: 20px; border-bottom-width: 1px; border-bottom-style: solid; border-bottom-color: #dec79e;" class="blockme left">
                      {{ include('company/sdta/templates/events.single.html',{row:row}) }}
                      </td>
                    {% else %}
                      
                      <td width="50%" valign="top" align="left" style="padding-left: 5px; padding-top: 20px; padding-bottom: 20px; border-bottom-width: 1px; border-bottom-style: solid; border-bottom-color: #dec79e;" class="blockme right"> 
                      {{ include('company/sdta/templates/events.single.html',{row:row}) }}
                      </td>
                    </tr>   
                    {% endif %}
                     
                      {% endfor %}
                    
                    {% else %}
                    <tr>
                      <td width="50%" valign="top" align="left" style="padding-right: 5px; padding-top: 20px; padding-bottom: 20px; border-bottom-width: 1px; border-bottom-style: solid; border-bottom-color: #dec79e;" class="blockme left">
                      {{ include('company/sdta/templates/events.single.html') }}
                      </td>
                      <td width="50%" valign="top" align="left" style="padding-left: 5px; padding-top: 20px; padding-bottom: 20px; border-bottom-width: 1px; border-bottom-style: solid; border-bottom-color: #dec79e;" class="blockme right"> 
                      {{ include('company/sdta/templates/events.single.html') }}
                      </td>
                    </tr>
                    {% endif %}
                    <!-- END: ROW --> 
                  </tbody>
                </table></td>
            </tr>
          </tbody>
        </table></td>
    </tr>
  </tbody>
</table>
{{ include('company/sdta/_controls.html', {controls:'events',colors:false}) }}
{% endblock %}