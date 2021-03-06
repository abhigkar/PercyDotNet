﻿const fs = require("fs");
const { spawn } = require("child_process");

// We need to change the path / command based on the platform they're using
const NPM_CMD = "npm.cmd";
const PERCY_CMD = `${process.cwd()}\\node_modules\\.bin\\percy.cmd`;
const exeCMD = `${process.cwd()}\\bin\\Debug\\percy-example-selenium.exe`; 
/**
 * Run tests by calling the percy executable and passing
 * the right mvn executable
 *
 */
function runTests() {
    const tests = spawn(exeCMD, ["test"], {
        stdio: "inherit",
        windowsVerbatimArguments: true
    });

    tests.on("close", () => {
        console.log("Tests completed!");
    });

    // Otherwise errors are given a 0 exit code
    tests.on("exit", code => {
        code !== 0 ? process.exit(code) : "";
    });
}

// If the dependencies aren't present then lets do the install for the user
if (!fs.existsSync(PERCY_CMD)) {
    const install = spawn(NPM_CMD, ["install"], {
        stdio: "inherit",
        windowsVerbatimArguments: true
    });

    install.on("close", () => {
        console.log(`Dependencies installed!`);
        runTests();
    });
} else {
    runTests();
}