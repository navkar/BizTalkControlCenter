<?xml version="1.0" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">	
<xsl:output method="html" indent="no"/>

  <xsl:template match="/">
    <html>
      <body>
        <xsl:call-template name="header" />
        <!-- Task -->
        <xsl:if test="count(DocumentElement/Task)>0">

         <table cellspacing="0" cellpadding="4" rules="all" border="1" style="color:#333333;width:100%;border-collapse:collapse;">
            <tr style="color:Gold;background-color:#525252;font-weight:bold;">
              <td>#</td>
              <td>Task</td>
              <td>Project</td>
              <td>Due On</td>
              <td>Status</td>
            </tr>

            <xsl:for-each select="DocumentElement/Task">
              <tr style="background-color:#FFFFCC;color:#525252;font-family:Verdana;font-size:10pt;font-weight:normal">
                <td>
                  <xsl:value-of select="./taskRef" />
                </td>
                <td>
                  <xsl:value-of select="./taskTitle" />
                </td>
                <td>
                  <xsl:value-of select="./ProjectName" />
                </td>
                <td>
                  <xsl:call-template name="formatDate">
                    <xsl:with-param name="dateTime" select="./taskDueDate" />
                  </xsl:call-template>
                </td>
                <td>
                  <xsl:value-of select="./taskStatus" />
                </td>
              </tr>
            </xsl:for-each>

          </table>
        </xsl:if>

        <br/>
      </body>
    </html>
	</xsl:template>

    <xsl:template name="header">

        <table align="center" width="100%" border="1" bgcolor="#525252" cellspacing="0" cellpadding="0" rules="all" style="color:gold;width:100%;border-collapse:collapse;font-family:Verdana;font-size:10pt;font-weight:normal">
            <tr>
                <td width="100%" valign="middle" align="center">
                  <B>BizTalk Control Center (BCC) - Task Management System</B>
                </td>
            </tr>
        </table>

    </xsl:template>

  <xsl:template name="formatDate">
    <xsl:param name="dateTime" />
    <xsl:variable name="date" select="substring-before($dateTime, 'T')" />
    <xsl:variable name="year" select="substring-before($date, '-')" />
    <xsl:variable name="month" select="substring-before(substring-after($date, '-'), '-')" />
    <xsl:variable name="day" select="substring-after(substring-after($date, '-'), '-')" />
    <xsl:value-of select="concat($month, '-', $day, '-', $year)" />
  </xsl:template>
  
  <xsl:template name="formatTime">
    <xsl:param name="dateTime" />
    <xsl:value-of select="substring-after($dateTime, 'T')" />
  </xsl:template>

</xsl:stylesheet>
