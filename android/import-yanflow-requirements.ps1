$ErrorActionPreference = "Stop"

$workspaceRoot = Split-Path -Parent $PSScriptRoot
$specPath = Join-Path $PSScriptRoot "SporeAccountingAndroid-需求规格说明.md"
$configPath = Join-Path $env:USERPROFILE ".codex\config.toml"

function Get-McpConfiguration {
    $config = Get-Content -LiteralPath $configPath -Encoding UTF8
    $start = [Array]::IndexOf($config, "[mcp_servers.sporeaccountingandroid]")
    if ($start -lt 0) {
        throw "sporeaccountingandroid MCP is not configured"
    }

    $section = $config[($start + 1)..([Math]::Min($start + 8, $config.Count - 1))]
    $url = (($section | Where-Object { $_ -match '^url\s*=' }) -replace '^url\s*=\s*"|"\s*$', '')
    $tokenReference = (($section | Where-Object { $_ -match '^bearer_token_env_var\s*=' }) -replace '^bearer_token_env_var\s*=\s*"|"\s*$', '')
    $token = [Environment]::GetEnvironmentVariable($tokenReference)

    # 兼容当前配置误将 Token 本身写入 bearer_token_env_var 的情况。
    if ([string]::IsNullOrWhiteSpace($token) -and $tokenReference.StartsWith("yf_mcp_")) {
        $token = $tokenReference
    }
    if ([string]::IsNullOrWhiteSpace($url) -or [string]::IsNullOrWhiteSpace($token)) {
        throw "sporeaccountingandroid MCP URL or token is unavailable"
    }

    return @{ Url = $url; Token = $token }
}

$mcp = Get-McpConfiguration
$headers = @{
    Authorization = "Bearer $($mcp.Token)"
    "MCP-Protocol-Version" = "2025-06-18"
    Accept = "application/json"
}
$requestId = 0

function Invoke-McpTool {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][hashtable]$Arguments
    )

    $script:requestId++
    $payload = @{
        jsonrpc = "2.0"
        id = $script:requestId
        method = "tools/call"
        params = @{ name = $Name; arguments = $Arguments }
    } | ConvertTo-Json -Depth 20 -Compress

    $response = Invoke-RestMethod -Method Post -Uri $mcp.Url -Headers $headers -ContentType "application/json" -Body $payload
    if ($response.error) {
        throw "MCP $Name failed: $($response.error | ConvertTo-Json -Depth 10 -Compress)"
    }
    if ($response.result.isError) {
        throw "MCP $Name failed: $($response.result.content[0].text)"
    }
    return $response.result.content[0].text | ConvertFrom-Json
}

function Get-RequirementPriority {
    param([string]$Code)

    if ($Code -match '^R[0-3]-') { return "P0" }
    if ($Code -in @("R5-04", "R8-01", "R8-02", "R8-03")) { return "P2" }
    return "P1"
}

$content = Get-Content -LiteralPath $specPath -Raw -Encoding UTF8
$iterationMatches = [regex]::Matches(
    $content,
    '(?ms)^## (?<section>\d+)\. Iteration (?<number>\d+)：(?<theme>[^\r\n]+)\r?\n(?<body>.*?)(?=^## \d+\.|\z)'
)
if ($iterationMatches.Count -ne 9) {
    throw "Expected 9 iterations in specification, found $($iterationMatches.Count)"
}

$existingIterations = (Invoke-McpTool -Name "list_iterations" -Arguments @{}).items
$existingRequirements = (Invoke-McpTool -Name "list_requirements" -Arguments @{}).items
$iterationIds = @{}
$createdIterations = 0
$createdRequirements = 0
$skippedIterations = 0
$skippedRequirements = 0

foreach ($iterationMatch in $iterationMatches) {
    $number = [int]$iterationMatch.Groups["number"].Value
    $theme = $iterationMatch.Groups["theme"].Value.Trim()
    $body = $iterationMatch.Groups["body"].Value
    $name = "Iteration $number：$theme"
    $goalMatch = [regex]::Match($body, '(?ms)^### [^\r\n]*迭代目标\r?\n(?<goal>.*?)(?=^### |\z)')
    $goal = $goalMatch.Groups["goal"].Value.Trim()

    $existing = $existingIterations | Where-Object { $_.name -eq $name } | Select-Object -First 1
    if ($existing) {
        $iterationIds[$number] = $existing.id
        $skippedIterations++
        Write-Output "SKIP iteration: $name"
    }
    else {
        $created = Invoke-McpTool -Name "create_iteration" -Arguments @{ name = $name; goal = $goal }
        $iterationIds[$number] = $created.iteration.id
        $createdIterations++
        Write-Output "CREATE iteration: $name -> $($created.iteration.id)"
    }

    $acceptanceMatch = [regex]::Match($body, '(?ms)^### [^\r\n]*验收标准\r?\n(?<criteria>.*?)(?=^### |\z)')
    $acceptanceCriteria = $acceptanceMatch.Groups["criteria"].Value.Trim()
    $requirementMatches = [regex]::Matches(
        $body,
        '(?ms)^#### (?<code>R\d+-\d+) (?<title>[^\r\n]+)\r?\n(?<description>.*?)(?=^#### |^### |\z)'
    )

    foreach ($requirementMatch in $requirementMatches) {
        $code = $requirementMatch.Groups["code"].Value.Trim()
        $titleText = $requirementMatch.Groups["title"].Value.Trim()
        $title = "$code $titleText"
        $description = $requirementMatch.Groups["description"].Value.Trim()
        $existingRequirement = $existingRequirements | Where-Object { $_.title -eq $title } | Select-Object -First 1

        if ($existingRequirement) {
            $skippedRequirements++
            Write-Output "SKIP requirement: $title"
            continue
        }

        $arguments = @{
            title = $title
            description = $description
            business_background = "所属迭代目标：$goal"
            user_value = "交付 $titleText，支撑 SporeAccounting Android 客户端的 $theme 能力。"
            acceptance_criteria = $acceptanceCriteria
            priority = Get-RequirementPriority -Code $code
            iteration_id = $iterationIds[$number]
            labels = @("Android", "SporeAccounting", $code, "Iteration-$number")
            source = "SporeAccountingAndroid-需求规格说明.md v1.0"
            target_version = "Iteration $number"
            requirement_type = "功能需求"
        }
        $created = Invoke-McpTool -Name "create_requirement" -Arguments $arguments
        $createdRequirements++
        Write-Output "CREATE requirement: $title -> $($created.id)"
    }
}

$finalIterations = Invoke-McpTool -Name "list_iterations" -Arguments @{}
$finalRequirements = Invoke-McpTool -Name "list_requirements" -Arguments @{}
Write-Output "SUMMARY created_iterations=$createdIterations skipped_iterations=$skippedIterations created_requirements=$createdRequirements skipped_requirements=$skippedRequirements final_iterations=$($finalIterations.total) final_requirements=$($finalRequirements.total)"
