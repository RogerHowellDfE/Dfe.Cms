import { cpSync, existsSync, mkdirSync, writeFileSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);
const projectRoot = join(__dirname, '..');

// Note: GOV.UK Frontend assets are provided by the C#/dotnet GovUk.Frontend.AspNetCore NuGet package
// and served from _content/GovUk.Frontend.AspNetCore/

// Copy DfE rebrand logo assets
// Note: At the time of writing, the GOV.UK rebrand logos (department-for-education_white.png
// and department-for-education_black.png) are NOT available via the dfe-frontend npm package.
// They must be downloaded separately from https://design.education.gov.uk/
// We store the canonical copies in src/assets/images/ (committed to git) and copy to wwwroot.
// If they don't exist in src/, we attempt to download them automatically.

const srcImageDir = join(projectRoot, 'src', 'assets', 'images');
const wwwrootImageDir = join(projectRoot, 'wwwroot', 'assets', 'images');

const rebrandLogos = [
  {
    filename: 'department-for-education_white.png',
    url: 'https://design.education.gov.uk/assets/images/rebrand/department-for-education_white.png'
  },
  {
    filename: 'department-for-education_black.png',
    url: 'https://design.education.gov.uk/assets/images/rebrand/department-for-education_black.png'
  }
];

// Ensure directories exist
if (!existsSync(srcImageDir)) {
  mkdirSync(srcImageDir, { recursive: true });
}
if (!existsSync(wwwrootImageDir)) {
  mkdirSync(wwwrootImageDir, { recursive: true });
}

// Download and copy rebrand logos
for (const logo of rebrandLogos) {
  const srcPath = join(srcImageDir, logo.filename);
  const destPath = join(wwwrootImageDir, logo.filename);

  // If source doesn't exist, download it
  if (!existsSync(srcPath)) {
    console.log(`⬇ Downloading ${logo.filename} from design.education.gov.uk...`);
    try {
      const response = await fetch(logo.url);
      if (!response.ok) {
        throw new Error(`HTTP ${response.status}`);
      }
      const buffer = await response.arrayBuffer();
      writeFileSync(srcPath, Buffer.from(buffer));
      console.log(`✓ Downloaded ${logo.filename}`);
    } catch (error) {
      console.error(`✗ Failed to download ${logo.filename}: ${error.message}`);
      continue;
    }
  }

  // Copy from src to wwwroot
  if (existsSync(srcPath)) {
    cpSync(srcPath, destPath);
  }
}

console.log('✓ DfE rebrand logo assets copied');
