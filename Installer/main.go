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

//go:embed MultiAssembly.dll
var multiAssemblyFile []byte

var ps = string(os.PathSeparator)

var ioReader = bufio.NewReader(os.Stdin)

// Change for different operating systems
var aviassemblyFolder = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Aviassembly"

func main() {
	if _, err := os.Stat(aviassemblyFolder); os.IsNotExist(err) {
		fmt.Println("Aviassembly folder does not exist")
		hangAndClose(1)
	}
	if _, err := os.Stat(aviassemblyFolder + ps + "BepInEx" + ps + "plugins" + ps + "MultiAssembly.dll"); !os.IsNotExist(err) {
		fmt.Println("Mod already installed. Would you like to uninstall it? (y/n)")
		if v, _ := ioReader.ReadString('\n'); v[0] == 'y' {
			err := os.Remove(aviassemblyFolder + ps + "BepInEx" + ps + "plugins" + ps + "MultiAssembly.dll")
			if err != nil {
				fmt.Println("Failed to uninstall MultiAssembly. Try to run the installer in administrator mode")
				hangAndClose(1)
			}
			fmt.Println("Successfully uninstalled MultiAssembly")
			hangAndClose(0)
		}
	}
	fmt.Println("Aviassembly folder found. Extracting...")
	extBepinex()
	extMod()
	fmt.Println("--------")
	fmt.Println("Successfully installed the MultiAssembly mod")
	fmt.Println("Please start and quit Aviassembly once to finish setup. The mod should be functioning afterwards.")
	hangAndClose(0)
}

func hangAndClose(code int) {
	fmt.Println("Press any key to close...")
	ioReader.ReadRune()
	os.Exit(code)
}
func extBepinex() {
	if _, err := os.Stat(aviassemblyFolder + ps + "BepInEx"); !os.IsNotExist(err) {
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
			fmt.Println("Cannot read internal zip file")
			hangAndClose(1)
		}
		dat, _ := io.ReadAll(zfile)
		os.WriteFile(aviassemblyFolder+string(os.PathSeparator)+strings.Join(segs, string(os.PathSeparator)), dat, 0777)
	}

}
func extMod() {
	if _, err := os.Stat(aviassemblyFolder + ps + "BepInEx" + ps + "plugins"); os.IsNotExist(err) {
		os.Mkdir(aviassemblyFolder+ps+"BepInEx"+ps+"plugins", 0777)
	}
	err := os.WriteFile(aviassemblyFolder+ps+"BepInEx"+ps+"plugins"+ps+"MultiAssembly.dll", multiAssemblyFile, 0777)
	if err != nil {
		fmt.Println("Failed to write MultiAssembly dll to the BepInEx plugins folder")
		hangAndClose(1)
	}
}
