const fs = require('fs');
const pdf = require('pdf-parse/lib/pdf-parse.js');

async function main() {
  const files = fs.readdirSync('.').filter(f => f.endsWith('.pdf'));
  if (files.length === 0) {
    console.log('No PDF files found');
    return;
  }
  const pdfFile = files[0];
  console.log('Reading:', pdfFile);
  const dataBuffer = fs.readFileSync(pdfFile);
  const data = await pdf(dataBuffer);
  console.log('=== PDF TEXT START ===');
  console.log(data.text);
  console.log('=== PDF TEXT END ===');
}

main().catch(console.error);
