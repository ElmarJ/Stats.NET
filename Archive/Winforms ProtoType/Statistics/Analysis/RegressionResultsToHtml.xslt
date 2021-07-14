<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes"
    doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"
    doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN" />
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en-US" lang="en-US">
      <head>
        <title>Regression Results</title>
      </head>
      <body>
        <h1>Regression Results</h1>
        <h2>Parameter Estimates</h2>
        <table>
          <caption>
            Least Square Estimates of the coefficients
          </caption>
          <thead>
            <tr>
              <th>Variable</th>
              <th>Coefficient</th>
            </tr>
          </thead>
          <tbody>
            <xsl:for-each select="RegressionResults/Coefficients/item">
              <tr>
                <td>
                  <xsl:value-of select="key/Variable/@Name"/>
                </td>
                <td>
                  <xsl:value-of select="value/double"/>
                </td>
              </tr>
            </xsl:for-each>
          </tbody>
        </table>
        <h2>Measures of fit</h2>
        <table>
          <thead>
            <tr>
              <td>Measure</td>
              <td>Result</td>
            </tr>
          </thead>
          <tr>
            <td>R<sup>2</sup></td>
            <td><xsl:value-of select="RegressionResults/RSquare" /></td>
          </tr>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>