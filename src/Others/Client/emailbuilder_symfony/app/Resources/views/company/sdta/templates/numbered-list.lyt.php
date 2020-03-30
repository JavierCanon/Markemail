{% extends 'company/sdta/container.html' %}
{% block body %}
  <table width="100%" cellspacing="0" cellpadding="0" border="0" class="contentarea twocol">
    <tbody>
      <tr>
        <td class="fullpad" bgcolor="#fddea6" background="http://image.exct.net/lib/fe6e15707166047a7715/m/1/sdta_nl_small_texture_tan.jpg" style="padding: 15px 30px;"><table width="100%" cellspacing="0" cellpadding="0" border="0">
            <tbody>
              <tr>
                <td colspan="3" style="font-size: 18px; letter-spacing: 0.3px; line-height: 24px; color: #005f86;"><font style="font-family: 'Arial Narrow', Helvetica, sans-serif;"> 
                  <!--[if (!mso 14)&(!mso 15)]><!--> 
                  <font style="font-family: Oswald, 'Arial Narrow', Helvetica, Arial, sans-serif;"> 
                  <!--<![endif]--> 
                  
                  <span class="editable">5 IDEA FORECAST</span> 
                  
                  <!--[if (!mso 14)&(!mso 15)]><!--> 
                  </font> 
                  <!--<![endif]--> 
                  </font></td>
              </tr>
              <tr>
                <td colspan="3">&nbsp;</td>
              </tr>
              <tr>
                <td colspan="3" style="font-size: 27px; line-height: 34px; color: #ef7622; padding: 0px 0px 16px;"><font style="font-family: 'Arial Narrow', Helvetica, sans-serif;"> 
                  <!--[if (!mso 14)&(!mso 15)]><!--> 
                  <font style="font-family: Oswald, 'Arial Narrow', Helvetica, Arial, sans-serif;"> 
                  <!--<![endif]--> 
                  
                  <span class="editable">SECTION HEADLINE HERE</span> 
                  
                  <!--[if (!mso 14)&(!mso 15)]><!--> 
                  </font> 
                  <!--<![endif]--> 
                  </font></td>
              </tr>
              <tr>
                <td colspan="3"><p style="font-family: Arial, sans-serif; font-size: 13px; color: #55565a; letter-spacing: 0.3px; margin: 0px; padding: 0px;"> <span class="editable">Sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt.</span> </p></td>
              </tr>
              <tr>
                <td colspan="3">
                
                <table width="100%" border="0" cellpadding="0" cellspacing="0" class="numbered-list">
               
                <tbody>
                <tr><td valign="top" class="numbered-list-container">
                
              {{ include('company/sdta/templates/numbered-list.part.html') }}
                  
                  </td></tr>
                  </tbody>
                </table>
                  
                </td>
              </tr>
              <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td valign="top">&nbsp;</td>
              </tr>
            </tbody>
          </table></td>
      </tr>

    </tbody>
  </table>
{{ include('company/sdta/_controls.html', {controls:'numbered-list',colors:true}) }}
{% endblock %}
