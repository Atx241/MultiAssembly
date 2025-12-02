package main

import (
	"archive/zip"
	"bufio"
	"bytes"
	_ "embed"
	"fmt"
	"io"
	"os"
	"strings"
)

//go:embed bepinex.zip
var bepinexFile []byte
var aviassemblyFolder = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Aviassembly"

func main() {
	if _, err := os.Stat(aviassemblyFolder); os.IsNotExist(err) {
		fmt.Println("Aviassembly folder does not exist")
		hangAndClose(1)
		return
	}
	fmt.Println("Aviassembly folder found")
	extBepinex()
	extMod()
	hangAndClose(0)
}

func hangAndClose(code int) {
	fmt.Println("Press any key to close...")
	bufio.NewReader(os.Stdin).ReadRune()
	os.Exit(code)
}
func extBepinex() {
	if _, err := os.Stat(aviassemblyFolder + "\\BepInEx"); !os.IsNotExist(err) {
		return
	}
	fmt.Println("BepInEx does not exist, creating...")
	zr, err := zip.NewReader(bytes.NewReader(bepinexFile), int64(len(bepinexFile)))
	if err != nil {
		fmt.Println("Failed to extract BepInEx file. Try to run the installer using administrator mode.")
		hangAndClose(1)
	}
	for _, f := range zr.File {
		fmt.Println("Extracting", f.Name)
		segs := strings.Split(f.Name, "/")
		for i := 0; i < len(segs)-1; i++ {
			folder := aviassemblyFolder + string(os.PathSeparator) + strings.Join(segs[:(i+1)], string(os.PathSeparator))
			if _, err = os.Stat(folder); os.IsNotExist(err) {
				fmt.Println("Creating folder", folder)
				err := os.Mkdir(folder, 0777)

				if err != nil {
					fmt.Println("Failed to create folder", folder)
					hangAndClose(1)
				}
			}
		}
		zfile, err := f.Open()
		if err != nil {
			fmt.Println("Cannot read internal zip file.")
			hangAndClose(1)
		}
		dat, _ := io.ReadAll(zfile)
		os.WriteFile(aviassemblyFolder+string(os.PathSeparator)+strings.Join(segs, string(os.PathSeparator)), dat, 0777)
	}

}
