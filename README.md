# DS_TextsMod_Helper

Following the console application [DS-Mod-CompareFmgCsv](https://github.com/FrenzMcJ0hns0n/DS-Mod-CompareFmgCsv)

The aim of this program is to ease translators work on Souls games mods.  
Basically it takes FMG files in input and loops through their data to generate a new file in output.

![v1.2 screenshot](https://i.imgur.com/UwTsLmL.png)

## How to use it

From top to bottom :
1) Drag/drop input files
2) Click the Action button of the desired usage (explanations and output preview will appear)
3) (Optional) Change values of the Options fields
4) Click the Execute button to generate the output file

## 2 usages : **Compare/Compile** & **Prepare/Comply**

### Compare mode

Create a new CSV file with exhaustive line-by-line diffchecker between two files.  
(See explanations within the program)

Output preview:
```
Text ID|File1 text|File2 text|Same?
TextID 1|File1 value1|File2 value1|true
TextID 2|File1 value2|File2 value2|false
...
```

### Prepare mode

Create a new FMG file, matching the data structure of a mod with data filled dynamically, to work on a new translation.  
(See explanations within the program)

Output preview:
```
Text ID|Value
TextID 1|value1
TextID 2|value2
...
```
