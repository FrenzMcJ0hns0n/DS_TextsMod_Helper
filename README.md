# DS_TextsMod_Helper

Following the console application [DS-Mod-CompareFmgCsv](https://github.com/FrenzMcJ0hns0n/DS-Mod-CompareFmgCsv)

Basically this program takes CSV files in input, then does some operations to generate other and bigger CSV files in output.
But it is designed with a much more specific use in mind : ease the work of translators around text mods for the Souls games.

![v1.02 screenshot](https://i.imgur.com/sa0Ma8t.png)

## How to use it

From top to bottom :
1) Drag/drop input files
2) Click the Action button of the desired usage (explanations about it and output preview will appear)
3) (Optional) Change values of the Options fields
4) Click the Execute button to generate the CSV output file

## 2 usages : **Compare/Compile** & **Prepare/Comply**

### Compare mode

You can use this as a way to create exhaustive line-by-line diffchecker between two files.

(See explanations within the program)

Output preview:
```
Text ID|File1 text|File2 text|Same?
TextID 1|File1 value1|File2 value1|true
TextID 2|File1 value2|File2 value2|false
...
```

### Prepare mode

You can use this as a way to prepare a file, matching the structure of a source mod file, with values inserted dynamically.

(See explanations within the program)

Output preview:
```
Text ID|Value
TextID 1|value1
TextID 2|value2
...
```
