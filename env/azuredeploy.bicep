@description('A 3-4 letter customer affix')
param clientAffix string

@description('A 2 letter designator for the environment')
param env string = 'Dv'

@description('The environment index - 0 is the default for the initial environment')
@minValue(0)
@maxValue(9)
param envIndex int = 0

@description('A 3-9 letter project affix')
param projectAffix string

@description('Set to true when the project affix is generic ie Remote Monitoring (rmtmon), traXsmart (txs), etc ')
param qualifyProjectAffix bool = false

var clientAffixTitleCase = '${toUpper(first(clientAffix))}${toLower(substring(clientAffix, 1, (length(clientAffix) - 1)))}'
var envIndex_var = ((envIndex > 0) ? envIndex : '')
var projectAffixTitleCase = '${toUpper(first(projectAffix))}${toLower(substring(projectAffix, 1, (length(projectAffix) - 1)))}'
var namingPrefix = '${env}${(qualifyProjectAffix ? '${clientAffixTitleCase}${toLower(projectAffixTitleCase)}' : projectAffixTitleCase)}${envIndex_var}'
var resourceNames = {
  sharedAppInsights: '${namingPrefix}ShrAi'
  sharedKeyVault: '${namingPrefix}ShrKv'
  apiCompute: '${namingPrefix}ShrAsp'
  appService: '${namingPrefix}MgmtApiAs'
  apiMgmt: '${namingPrefix}ShrApiMgmt'
}

resource appService 'Microsoft.Web/sites@2020-06-01' = {
  name: resourceNames.appService
  location: resourceGroup().location
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    enabled: true
    serverFarmId: resourceId('Microsoft.Web/serverfarms', resourceNames.apiCompute)
    clientAffinityEnabled: true
    httpsOnly: true
    siteConfig: {
      appSettings: [
        {
          name: 'ApplicationInsights:InstrumentationKey'
          value: reference(resourceId('Microsoft.Insights/components', resourceNames.sharedAppInsights), '2014-04-01').instrumentationKey
        }
      ]
    }
  }
}

resource appService_config 'Microsoft.Web/sites/config@2021-02-01' = {
  parent: appService
  name: 'web'
  properties: {
    ipSecurityRestrictions: [
      {
        ipAddress: '${reference(resourceId('Microsoft.ApiManagement/service', resourceNames.apiMgmt), '2019-01-01').publicIPAddresses[0]}/32'
        action: 'Allow'
        priority: 1
        name: 'API Management access'
      }
      {
        ipAddress: 'Any'
        action: 'Deny'
        priority: 300
        name: 'Deny all'
        description: 'Deny all access'
      }
    ]
  }

}

output appService string = resourceNames.appService
output appServiceHostName string = appService.properties.defaultHostName
