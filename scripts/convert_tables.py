import re
from pathlib import Path

DOCS_DIR = Path("docs")


def parse_row(row):
    cells = row.strip().strip("|").split("|")
    return [c.strip() for c in cells]


def build_html_table(lines):
    header = parse_row(lines[0])
    rows = [parse_row(line) for line in lines[2:]]

    html = ["<table>", "<thead>", "<tr>"]
    for h in header:
        html.append("<th>" + h + "</th>")
    html.append("</tr>")
    html.append("</thead>")
    html.append("<tbody>")
    for row in rows:
        html.append("<tr>")
        for cell in row:
            html.append("<td>" + cell + "</td>")
        html.append("</tr>")
    html.append("</tbody>")
    html.append("</table>")
    return "\n".join(html)


def markdown_table_to_html(md_text):
    lines = md_text.split("\n")
    output = []
    i = 0

    while i < len(lines):
        line = lines[i]

        if "|" in line and i + 1 < len(lines) and re.match(r"^\s*\|[\s\-:|]+\|\s*$", lines[i + 1]):
            table_lines = [line]
            j = i + 1
            while j < len(lines) and "|" in lines[j]:
                table_lines.append(lines[j])
                j += 1

            html_table = build_html_table(table_lines)
            output.append(html_table)
            i = j
        else:
            output.append(line)
            i += 1

    return "\n".join(output)


def process_file(path):
    content = path.read_text(encoding="utf-8")
    new_content = markdown_table_to_html(content)

    if new_content != content:
        path.write_text(new_content, encoding="utf-8")
        return True
    return False


def main():
    count = 0
    for md_file in DOCS_DIR.rglob("*.md"):
        if process_file(md_file):
            print("OK: " + str(md_file))
            count += 1

    print("\n--- Total: " + str(count) + " arquivos com tabelas convertidas ---")


if __name__ == "__main__":
    main()