import pymupdf
import os
import sys
import io

sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

pdf_files = [f for f in os.listdir('.') if f.endswith('.pdf')]
pdf_file = pdf_files[0]

doc = pymupdf.open(pdf_file)
with open('saoke_output.txt', 'w', encoding='utf-8') as f:
    for page_num in range(len(doc)):
        page = doc[page_num]
        f.write(f"\n=== PAGE {page_num + 1} ===\n")
        f.write(page.get_text())

doc.close()
print(f"Done. Output saved to saoke_output.txt ({len(doc)} pages)")
