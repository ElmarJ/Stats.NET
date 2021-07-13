<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes"
    doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"
    doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN" />
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en-US" lang="en-US">
      <head>
        <title>Correlation Results</title>
      </head>
      <body>
        <h1>Correlation Results</h1>
        <h2>Correlation Matrix</h2>
        <table>
          <caption>
            Pearsons R value for correlation between two variables
          </caption>
          <tr>
            <th></th>
            <xsl:for-each select="CorrelationResults/Variable">
              <th>
                <xsl:value-of select="@Name"/>
              </th>
            </xsl:for-each>
          </tr>
          <xsl:for-each select="CorrelationResults/Variable">
            <tr>
              <th>
                <xsl:value-of select="@Name"/>
              </th>
              <xsl:for-each select="Correlation">
                <td>
                  <xsl:value-of select="."/>
                </td>
              </xsl:for-each>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>