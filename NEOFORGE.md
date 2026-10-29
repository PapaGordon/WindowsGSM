# Minecraft: NeoForge

Select **Minecraft: NeoForge** in Install or Import. This is a separate built-in server beside **Minecraft: All the Mods 10 (ATM10)** and **Minecraft: Java Edition Server**. NeoForge starts without a modpack; place compatible server mods in `mods/` yourself. It shares the existing ATM10 Java console, shutdown and properties handling. ATM10 keeps its server-pack installation and updates.

## Versions and Java

`serverfiles/neoforge-version.txt` selects the NeoForge version:

- `21.1` (default): latest stable NeoForge for Minecraft 1.21.1.
- Another family such as `21.4`: latest stable within that family.
- An exact Maven version such as `21.1.265`: pin that release. Exact beta versions are allowed when explicitly selected.

An imported runtime uses its own family by default. Updates never jump to another Minecraft family unless you edit the selector. NeoForge version numbers are not Minecraft version numbers. Use the [official version list](https://maven.neoforged.net/releases/net/neoforged/neoforge/) when selecting a build. Changing the family requires checking world and mod compatibility yourself.

The plugin checks for a matching 64-bit Java major: Java 17 for NeoForge 20.2-20.4, Java 21 for 20.5-21.x, Java 25 for 26.x. Unsupported future families fail with an error. Set `JAVA_HOME` or add the matching Java installation to `PATH`. Common JDK installation directories are also checked. Java is not downloaded automatically.

Install asks for Minecraft EULA acceptance and downloads the installer from the official NeoForged Maven. Its SHA-256 is checked against the Maven checksum before `--installServer` runs. The installer still needs internet access to download its runtime dependencies. No CurseForge pack is needed.

## Settings and updates

Adjust RAM and Java options in `user_jvm_args.txt`. Additional WGSM parameters are game arguments, matching ATM10. The selected ports and player limit are applied to `server.properties`; an existing MOTD is kept. Auto Firewall uses separate TCP game and UDP query rules for this server instance, without a global Java rule.

Stop the server before updating. Preparation makes a full copy of `serverfiles` beside the live folder and installs fresh runtime libraries there. Worlds, mods, configuration, lists and existing JVM arguments are retained. Allow enough disk space for another complete server copy. A successful activation retains the previous `serverfiles` in `neoforge-update-backups`. Failed preparation leaves the live files untouched and keeps the prepared directory for inspection. A recovery marker blocks startup if an interrupted activation needs manual attention.

Import requires `user_jvm_args.txt` and an installed `libraries/net/neoforged/neoforge/<version>/win_args.txt`. If several runtimes exist, set `.wgsm-neoforge-version.txt` to the intended installed version. ATM10 folders with its version marker are rejected; use the ATM10 entry for those.

Local fixture checks cover version selection, Java requirements, import, configuration preservation, update activation/failure, locks and registration. On 2026-10-29, 39 fixture checks passed and the official NeoForge 21.1.265 installer was separately downloaded, checksum-verified and run with Java 21 in an isolated directory. The resulting runtime passed import validation. A live Minecraft session and production-server deployment are not part of those checks.

References: [NeoForge server installation](https://docs.neoforged.net/user/docs/server/), [Java requirements](https://docs.neoforged.net/user/docs/), [Java 25 migration](https://neoforged.net/news/26.1release/).
