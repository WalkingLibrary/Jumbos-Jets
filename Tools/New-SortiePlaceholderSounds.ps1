# Original synthesized placeholders; no external audio or libraries required.
$ErrorActionPreference = 'Stop'
$soundOutput = Join-Path (Split-Path -Parent $PSScriptRoot) 'JumbosJets/Sounds/Sorties'
New-Item -ItemType Directory -Force -Path $soundOutput | Out-Null
$sampleRate = 22050

foreach ($clip in @(@{ Name = 'StrafeFire.wav'; Seconds = 0.65; Tail = $false },
                   @{ Name = 'StrafeTail.wav'; Seconds = 2.5; Tail = $true })) {
    $sampleCount = [int]($sampleRate * $clip.Seconds)
    $writer = [IO.BinaryWriter]::new([IO.File]::Create((Join-Path $soundOutput $clip.Name)))
    try {
        $writer.Write([Text.Encoding]::ASCII.GetBytes('RIFF'))
        $writer.Write([int](36 + $sampleCount * 2))
        $writer.Write([Text.Encoding]::ASCII.GetBytes('WAVEfmt '))
        $writer.Write([int]16)
        $writer.Write([int16]1) # PCM
        $writer.Write([int16]1) # mono
        $writer.Write([int]$sampleRate)
        $writer.Write([int]($sampleRate * 2))
        $writer.Write([int16]2)
        $writer.Write([int16]16)
        $writer.Write([Text.Encoding]::ASCII.GetBytes('data'))
        $writer.Write([int]($sampleCount * 2))
        $rng = [Random]::new(30)
        $lowNoise = 0.0
        for ($i = 0; $i -lt $sampleCount; $i++) {
            $t = $i / $sampleRate
            $noise = $rng.NextDouble() * 2 - 1
            $lowNoise = 0.92 * $lowNoise + 0.08 * $noise
            $fade = [Math]::Min(1, $t / 0.015) * [Math]::Pow([Math]::Max(0, 1 - $t / $clip.Seconds), 1.5)
            if ($clip.Tail) {
                $value = 0.25 * $lowNoise + 0.08 * [Math]::Sin(2 * [Math]::PI * (110 * $t - 15 * $t * $t))
            } else {
                $pulse = 0.3 + 0.7 * [Math]::Max(0, [Math]::Sin(2 * [Math]::PI * 32 * $t))
                $value = $pulse * (0.18 * $noise + 0.08 * [Math]::Sin(2 * [Math]::PI * 85 * $t))
            }
            # Keep audible headroom before the SoundDef/game volume multipliers.
            $sample = [Math]::Max(-0.9, [Math]::Min(0.9, $value * $fade * 3))
            $writer.Write([int16]($sample * 32767))
        }
    } finally {
        $writer.Dispose()
    }
    Write-Output "Generated $($clip.Name)"
}
