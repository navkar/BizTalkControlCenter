<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:om="http://schemas.microsoft.com/BizTalk/2003/DesignerData">	
  <xsl:param name="OrchName" />
  
  <xsl:output method="html" indent="no"/>

	<xsl:template match="/">
			<div>
           <xsl:call-template name="header" />
      		 <!-- Code -->
			     <xsl:apply-templates />
			</div>
	</xsl:template>

  <xsl:template name="header">
      <table width="100%" border="0" cellpadding="0" cellspacing="0">
          <tr>
                <td width="100%" valign="middle" class="TableHeader">
                  <nobr>
                      <xsl:value-of select="$OrchName"/> [XLANG]
                  </nobr>
              </td>
          </tr>
      </table>
    <br/>
  </xsl:template>
	
	<xsl:template match="//om:Element[@Type='VariableAssignment']">
        <table width="98%">
            <tr>
                <td>
                    <xsl:call-template name="CodeElement">
                        <xsl:with-param name="Caption">Expression Shape</xsl:with-param>
                        <xsl:with-param name="Code">
                            <xsl:value-of select="om:Property[@Name='Expression']/@Value" />
                        </xsl:with-param>
                    </xsl:call-template>
                </td>
            </tr>
        </table>
        <br/>
	</xsl:template>
	
	<xsl:template match="//om:Element[@Type='MessageAssignment']">
        <table width="98%">
                <tr>
                    <td>
                        <xsl:call-template name="CodeElement">
                            <xsl:with-param name="Caption">Message Assignment Shape</xsl:with-param>
                            <xsl:with-param name="Code">
                                <xsl:value-of select="om:Property[@Name='Expression']/@Value" />
                            </xsl:with-param>
                        </xsl:call-template>
                    </td>
                </tr>
            </table>
            <br/>
        </xsl:template>

  <xsl:template match="//om:Element[@Type='Send']">
      <table width="98%">
          <tr>
              <td>
                  <xsl:call-template name="CodeElement">
                      <xsl:with-param name="Caption">Send Shape</xsl:with-param>
                      <xsl:with-param name="Desc">
                          <xsl:value-of select="om:Property[@Name='AnalystComments']/@Value" />
                      </xsl:with-param>
                      <xsl:with-param name="Code">Sends message '<xsl:value-of select="om:Property[@Name='MessageName']/@Value" />' to port '<xsl:value-of select="om:Property[@Name='PortName']/@Value" />'
                      </xsl:with-param>
                  </xsl:call-template>
              </td>
          </tr>
      </table>
      <br/>
  </xsl:template>

  <xsl:template match="//om:Element[@Type='Receive']">
      <table width="98%">
      <tr>
          <td>
              <xsl:call-template name="CodeElement">
                  <xsl:with-param name="Caption">Receive Shape</xsl:with-param>
                  <xsl:with-param name="Desc">
                      <xsl:value-of select="om:Property[@Name='AnalystComments']/@Value" />
                  </xsl:with-param>
                  <xsl:with-param name="Code">Receives message '<xsl:value-of select="om:Property[@Name='MessageName']/@Value" />' from port '<xsl:value-of select="om:Property[@Name='PortName']/@Value" />'
                  </xsl:with-param>
              </xsl:call-template>
          </td>
      </tr>
  </table>
  <br/>
</xsl:template>

  <xsl:template name="CodeElement">

      <xsl:param name="Caption" />
      <xsl:param name="Desc" />
      <xsl:param name="Code" />

      <table border="0" cellspacing="0" cellpadding="0" width="98%" align="left">
          <tr>
              <td width="1">
                  <xsl:element name="A">
                      <xsl:attribute name="ID">
                          <xsl:value-of select="@OID" />
                      </xsl:attribute>
                  </xsl:element>
              </td>
              <td>

                  <table border="1" cellspacing="0" cellpadding="0" width="100%" style="border: 1px;" align="left">
                      <tr>
                          <td>
                              <table border="0" cellspacing="0" cellpadding="2" width="100%">
                                  <tr>
                                      <td class="tableTitle" bgcolor="#c0c0c0" color="#DADBFC">
                                          <xsl:value-of select="$Caption" />: <xsl:value-of select="om:Property[@Name='Name']/@Value" />
                                      </td>
                                  </tr>
                                  <xsl:if test="string-length($Desc)>0">
                                      <tr>
                                          <td class="tableData"><xsl:value-of select="$Desc" /></td>
                                      </tr>
                                  </xsl:if>
                                  <xsl:if test="string-length($Code)>0">
                                      <tr>
                                          <td class="tableData" style="font-size=8pt; font-family: courier new;" bgcolor="#ECEAEC"><pre style="font-size=8pt;"><xsl:value-of select="$Code" /></pre></td>
                                      </tr>
                                  </xsl:if>
                              </table>
                          </td>
                      </tr>
                  </table>

              </td>
          </tr>
      </table>
        
  </xsl:template>
	
	<xsl:template match="text()">
	</xsl:template>

</xsl:stylesheet>
