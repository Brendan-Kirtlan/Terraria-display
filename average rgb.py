from PIL import Image
import os

def calculate_average_rgb(image_path):
    with Image.open(image_path) as img:
        img = img.convert("RGB")
        pixels = list(img.getdata())
        avg_rgb = (
            sum(pixel[0] for pixel in pixels) // len(pixels),
            sum(pixel[1] for pixel in pixels) // len(pixels),
            sum(pixel[2] for pixel in pixels) // len(pixels)
        )
        return avg_rgb

def print_average_rgb_values(input_folder):
    files = os.listdir(input_folder)
    #text file where you want avg rgb values printed in the table format
    text_file = r""

    for file_name in files:
        if file_name.lower().endswith(".png"):
            image_path = os.path.join(input_folder, file_name)

            avg_rgb = calculate_average_rgb(image_path)

            print(f"File: {file_name}, Average RGB: {avg_rgb}")
            appended_text = "(TileID." + file_name + ", new byte[] {" + str(avg_rgb[0]) + "," + str(avg_rgb[1]) + "," + str(avg_rgb[2]) + "}),\n"
            append_to_file(text_file, appended_text)

def append_to_file(file_path, content_to_append):
    try:
        with open(file_path, 'a') as file:
            file.write(content_to_append)
            print(f"Appended '{content_to_append}' to {file_path}")
    except IOError as e:
        print(f"Error: Unable to append to {file_path}. {e}")

if __name__ == "__main__":
    #Set your input folder containing PNG images
    input_folder = r""

    print_average_rgb_values(input_folder)
