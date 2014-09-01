<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">	

<xsl:output method="html" indent="no"/>

	<xsl:template match="/">
				<div>

            <xsl:call-template name="header" /> 

            <table class="tableData">
            <tr>
                <td></td>
                <td class="tableTitle" valign="top">Application:</td>
                <td class="tableData">
                    <xsl:value-of select="BizTalkBaseObject/ApplicationName" />
                </td>
            </tr>
			<tr>
			    <td width="10"></td>
			    <td width="145" class="tableTitle"><nobr>Parent Assembly:</nobr></td>
			    <td class="tableData">
			        <nobr>
                             <xsl:value-of select="BizTalkBaseObject/ParentAssembly/Name" />
                    </nobr>
                </td>
            </tr>
            <tr>
                <td></td>
			    <td class="tableTitle">Host:</td>
			    <td class="tableData">
			        <xsl:choose>
			            <xsl:when test="string-length(BizTalkBaseObject/Host/Name)>0">       
			    					        <xsl:value-of select="BizTalkBaseObject/Host/Name" />
				        </xsl:when>
				        <xsl:otherwise><xsl:value-of select="BizTalkBaseObject/Host/Name" /></xsl:otherwise>
				    </xsl:choose>
				</td>
			</tr>			
			</table>
			
			<!-- Variables -->		
			<xsl:if test="count(BizTalkBaseObject/Variables/Variable)>0">	
			<BR/>
			    <Table class="tableData">
			    <tr>
			        <td width="10"></td>
			        <td class="PageTitle3" colspan="10">Orchestration Variables</td>
			    </tr>
			    <tr>
            <td></td>
            <td></td>
            <td class="tableTitle">Name</td>
			        <td width="10"></td>
			        <td class="tableTitle">Type</td>
			        <td width="10"></td>
			        <td class="tableTitle">Scope</td>
			        <td width="10"></td>
			        <td class="tableTitle">Initial Value</td>
			    </tr>
			    
			    <xsl:for-each select="BizTalkBaseObject/Variables/Variable">
			        <tr>
                <td></td>
                <td></td>
                <td><xsl:value-of select="./Name" /></td>
			            <td></td>
			            <td><xsl:value-of select="./TypeName" /></td>
			            <td></td>
			            <td><nobr><xsl:value-of select="./Scope" /></nobr></td>
			            <td></td>
			            <td><xsl:value-of select="./InitialValue" /></td>
			            <td></td>
			        </tr>
			    </xsl:for-each>
			    
			    </Table>
			</xsl:if>
			
			<!-- Message -->		
			<xsl:if test="count(BizTalkBaseObject/Messages/Message)>0">	
			<BR/>
			    <Table class="tableData">
			    <tr>
			        <td width="10"></td>
			        <td class="PageTitle3" colspan="9">Orchestration Messages</td>
			    </tr>
			    <tr>
			        <td></td>
			        <td></td>
			        <td class="tableTitle">Name</td>
			        <td width="10"></td>
			        <td class="tableTitle">Type</td>
			        <td width="10"></td>
			        <td class="tableTitle">Scope</td>
			    </tr>
			    
			    <xsl:for-each select="BizTalkBaseObject/Messages/Message">
			        <tr>
			            <td></td>
						      <td></td>
			            <td><xsl:value-of select="./Name" /></td>
			            <td></td>
			            <td><xsl:value-of select="./TypeName" /></td>
			            <td></td>
			            <td><nobr><xsl:value-of select="./Scope" /></nobr></td>
			        </tr>
			    </xsl:for-each>
			    
			    </Table>
			</xsl:if>

                <!-- Multi part Message -->
                <xsl:if test="count(BizTalkBaseObject/MultiPartMessages/MultiPartMessage)>0">
                    <BR/>
                    <Table class="tableData" border="0">
                        <tr>
                            <td width="10"></td>
                            <td class="PageTitle3" colspan="9">Multi-Part Messages</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td></td>
                            <td class="tableTitle">Name</td>
                            <td width="10"></td>
                            <td class="tableTitle">
                                <nobr>Type Modifier</nobr>
                            </td>
                            <td width="10"></td>
                            <td class="tableTitle">
                                <nobr>Part Name</nobr>
                            </td>
                            <td width="10"></td>
                            <td class="tableTitle">
                                <nobr>Is Body Part</nobr>
                            </td>
                            <td width="10"></td>
                            <td class="tableTitle">
                                <nobr>Class Name</nobr>
                            </td>
                        </tr>

                        <xsl:for-each select="BizTalkBaseObject/MultiPartMessages/MultiPartMessage">
                            <tr>
                              <td></td>
                              <td></td>
                                <td>
                                    <xsl:value-of select="./Name" />
                                </td>
                                <td></td>
                                <td>
                                    <xsl:value-of select="./Modifier" />
                                </td>
                                <td></td>

                                <xsl:for-each select="./Parts/MultiPartMessage">
                                    <xsl:if test="position()=1">
                                        <td class="tableData">
                                            <nobr>
                                                <xsl:value-of select="./Name" />
                                            </nobr>
                                        </td>
                                        <td></td>
                                        <td class="tableData">
                                            <nobr>
                                                <xsl:value-of select="./IsBodyPart" />
                                            </nobr>
                                        </td>
                                        <td></td>
                                        <td class="tableData">
                                            <nobr>
                                                <xsl:value-of select="./ClassName" />
                                            </nobr>
                                        </td>
                                    </xsl:if>
                                    <xsl:if test="position()>1">
                                        <tr>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td class="tableData">
                                                <nobr>
                                                    <xsl:value-of select="./Name" />
                                                </nobr>
                                            </td>
                                            <td></td>
                                            <td class="tableData">
                                                <nobr>
                                                    <xsl:value-of select="./IsBodyPart" />
                                                </nobr>
                                            </td>
                                            <td></td>
                                            <td class="tableData">
                                                <nobr>
                                                    <xsl:value-of select="./ClassName" />
                                                </nobr>
                                            </td>
                                        </tr>
                                    </xsl:if>
                                </xsl:for-each>

                            </tr>
                        </xsl:for-each>

                    </Table>
                </xsl:if>
			
			<!-- Called Rules Policies -->		
			<xsl:if test="count(BizTalkBaseObject/CalledRules/Rule)>0">	
			<BR/>
			    <Table class="tableData">
			    <tr>
			        <td width="10"></td>
			        <td class="PageTitle3" colspan="9">Rules engine policies</td>
			    </tr>
			    <tr>
			        <td></td>
			        <td class="tableTitle">Policy Name</td>
			    </tr>
			    
			    <xsl:for-each select="BizTalkBaseObject/CalledRules/Rule">
			        <tr>
			            <td></td>
			            <td><xsl:value-of select="." /></td>
			        </tr>
			    </xsl:for-each>
			    
			    </Table>
			</xsl:if>
			
			<!-- Transforms -->		
			<xsl:if test="count(BizTalkBaseObject/Transforms/Transform)>0">	
			<BR/>
			    <Table class="tableData">
			    <tr>
			        <td width="10"></td>
			        <td class="PageTitle3" colspan="2">Maps</td>
			    </tr>
			    <tr>
			        <td></td>
			        <td class="tableTitle">Transform Name</td>
			    </tr>
			    
			    <xsl:for-each select="BizTalkBaseObject/Transforms/Transform">
			        <tr>
			            <td></td>
			            <td>
                       <xsl:value-of select="Name" />
                  </td>
			        </tr>
			    </xsl:for-each>
			    
			    </Table>
			</xsl:if>
			
			<!-- Correlation Sets -->		
			<xsl:if test="count(BizTalkBaseObject/CorrelationSets/CorrelationSet)>0">	
			<BR/>
			    <Table class="tableData">
			    <tr>
			        <td width="10"></td>
			        <td class="PageTitle3" colspan="8">Orchestration correlation sets</td>
			    </tr>
			    <tr>
			        <td></td>
			        <td class="tableTitle">Name</td>
			        <td width="10"></td>
			        <td class="tableTitle">Type</td>
			        <td width="10"></td>
			        <td class="tableTitle">Scope</td>
			        <td width="10"></td>
			        <td class="tableTitle">Description</td>
			    </tr>

			    <xsl:for-each select="BizTalkBaseObject/CorrelationSets/CorrelationSet">
			        <tr>
			            <td></td>
			            <td><xsl:value-of select="./Name" /></td>
			            <td></td>
			            <td>							     
					            <xsl:value-of select="TypeName" />
				          </td>
			            <td></td>
			            <td><nobr><xsl:value-of select="./Scope" /></nobr></td>
			            <td></td>
			            <td><nobr><xsl:value-of select="./Description" /></nobr></td>
			            <td></td>
			        </tr>
			    </xsl:for-each>
			    
			    </Table>
			</xsl:if>
			
			<!-- Ports -->		
			<xsl:if test="count(BizTalkBaseObject/Ports/OrchestrationPort)>0">	
			<BR/>
			    <Table class="tableData">
			    <tr>
			        <td width="10"></td>
			        <td class="PageTitle3" colspan="4">Orchestration Ports</td>
			    </tr>
			    <tr>
			        <td></td>
			        <td class="tableTitle">Name</td>
			        <td width="10"></td>
			        <td class="tableTitle">Bound To</td>
			    </tr>
    			
			    <xsl:for-each select="BizTalkBaseObject/Ports/OrchestrationPort">
			        <tr>
			            <td></td>
			            <td><xsl:value-of select="./Name" /></td>
			            <td></td>
			            <td>
			                <xsl:choose>
			                    <xsl:when test="string-length(SendPortName/Name)>0">
			                        <xsl:choose>
			                            <xsl:when test="string-length(./SendPortName/Id)>0"> 
					                        <xsl:value-of select="./SendPortName/Name" />
				                        </xsl:when>
				                        <xsl:otherwise><xsl:value-of select="./SendPortName/Name" /></xsl:otherwise>
				                    </xsl:choose>
				                </xsl:when>
			                    <xsl:when test="string-length(SendPortGroupName/Name)>0">
			                        <xsl:choose>
			                            <xsl:when test="string-length(./SendPortGroupName/Id)>0"> 
					                        <xsl:value-of select="./SendPortGroupName/Name" />
				                        </xsl:when>
				                        <xsl:otherwise><xsl:value-of select="./SendPortGroupName/Name" /></xsl:otherwise>
				                    </xsl:choose>
				                </xsl:when>
			                    <xsl:when test="string-length(ReceivePortName/Name)>0">
			                        <xsl:choose>
			                            <xsl:when test="string-length(./ReceivePortName/Id)>0"> 
					                        <xsl:value-of select="./ReceivePortName/Name" />
				                        </xsl:when>
				                        <xsl:otherwise><xsl:value-of select="./ReceivePortName/Name" /></xsl:otherwise>
				                    </xsl:choose>
				                </xsl:when>
				                <xsl:otherwise>&lt;Un-Bound&gt;</xsl:otherwise>
				            </xsl:choose>
				        </td>      
			        </tr>
			    </xsl:for-each>
			    </Table>
			</xsl:if>

                <!-- Role Links -->
                <xsl:if test="count(BizTalkBaseObject/RoleLinks/RoleLink)>0">
                    <BR/>
                    <Table class="tableData">
                        <tr>
                            <td width="10"></td>
                            <td class="PageTitle3" colspan="8">Orchestration Role links</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td></td>
                            <td class="tableTitle">Name</td>
                            <td width="10"></td>
                            <td class="tableTitle">Provider</td>
                            <td width="10"></td>
                            <td class="tableTitle">Consumer</td>
                            <td width="10"></td>
                            <td class="tableTitle">Type</td>
                        </tr>

                        <xsl:for-each select="BizTalkBaseObject/RoleLinks/RoleLink">
                            <tr>
                                <td></td>
                                <td>
                                    <xsl:value-of select="./Name" />
                                </td>
                                <td></td>
                                <td>
                                        <xsl:value-of select="ProviderRole/Name" />
                                </td>
                                <td></td>
                                <td>
                                        <xsl:value-of select="ConsumerRole/Name" />
                                </td>
                                <td></td>
                                <td>
                                    <nobr>
                                        <xsl:value-of select="TypeName" />
                                    </nobr>
                                </td>
                                <td></td>
                            </tr>
                        </xsl:for-each>

                    </Table>
                </xsl:if>
			
			<!-- Invoked Orchestrations -->		
			<xsl:if test="count(BizTalkBaseObject/InvokedOrchestrations/Orchestration)>0">	
			<BR/>
			    <Table class="tableData">
			    <tr>
			        <td width="10"></td>
			        <td class="PageTitle3" colspan="2">Invoked Orchestrations</td>
			    </tr>
    			
			    <xsl:for-each select="BizTalkBaseObject/InvokedOrchestrations/Orchestration">
			        <tr>
			            <td></td>
			            <td class="tableData"><xsl:value-of select="./Name" /></td>      
			        </tr>
			    </xsl:for-each>
			    </Table>
			</xsl:if>
      <br/>
			</div>
	</xsl:template>

    <xsl:template name="header">

        <table width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td width="100%" valign="middle" class="TableHeader">
                    <nobr>
                        <xsl:value-of select="./BizTalkBaseObject/Name"/> [Declarations]
                    </nobr>
                </td>
            </tr>
        </table>
        <BR/>

    </xsl:template>

</xsl:stylesheet>
