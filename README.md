# DS_TextsMod_Helper

Following the console application [DS-Mod-CompareFmgCsv](https://github.com/FrenzMcJ0hns0n/DS-Mod-CompareFmgCsv)

A modding tool designed to assist translators of Souls games mods.  
Basically it takes FMG files in input and loops through their text entries to generate new files in output.

![v1.2 screenshot](https://i.imgur.com/FsIQz3F.png)

## How to use it

From top to bottom :
1) Drag/drop input files
2) Click the Action button of the desired usage (explanations and output preview will appear)
3) (Optional) Change values of the Options fields
4) Click the Execute button to generate the output file

## File processing in 2 modes

### Compare mode

**Principle: Process 2 FMG input files to build line by line diffcheckers in CSV format**  
Screenshot: [DSTMH v1.2 - Compare mode](https://i.imgur.com/QvYqbUR.png)  
Output preview:
```
Text ID|File1 text|File2 text|Same?
TextID 1|File1 value1|File2 value1|true
TextID 2|File1 value2|File2 value2|false
...
```

### Prepare mode

**Principle: Process 3 FMG input files to prepare a FMG file for a new translation**  
Screenshot: [DSTMH v1.2 - Prepare mode](https://i.imgur.com/H7VGNpz.png)  
Output preview:
```
Text ID|Value
TextID 1|value1
TextID 2|value2
...
```
