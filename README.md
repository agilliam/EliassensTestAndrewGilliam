# Order Processing System

This console application is a C# program designed to efficiently handle the processing of a large order list. The primary problem it addresses is finding all orders where the due date is today and returning them to the calling method for processing. Emphasis is placed on performance, with a goal to process orders as quickly as possible.

## Problem Statement

You have a list of 1 million orders, and the objective is to identify and retrieve orders with a due date set for today. These orders must be promptly returned to the calling method for immediate processing. The key challenge is to ensure optimal performance, as timely processing is a critical requirement.

## Solution Overview

### Order Generation and Storage - For testing only

1. **CSV File Handling:**
   - The program generates a CSV file named `Orders.csv` to store order data.
   - If the file already exists, it reads orders from the CSV file; otherwise, it generates and saves a new set of orders.

### Order Processing

2. **Order Due Today:**
   - Orders due today are identified efficiently by comparing the due date of each order with the current date.
   - The `GetOrdersDueToday` method filters and retrieves orders due today.

3. **Concurrency and Asynchronous Processing:**
   - The program uses asynchronous programming to handle the large dataset efficiently.
   - The `async` and `await` keywords are employed to ensure non-blocking execution, allowing for better utilization of system resources.

4. **Parallel Processing:**
   - Asynchronous processing enables parallelism, allowing the program to process multiple orders simultaneously, enhancing performance.

### Performance Considerations

5. **Random Order Generation:**
   - The application generates a large number of random orders, simulating a real-world scenario.
   - Random order dates and due dates contribute to diverse data that challenges the program's processing capabilities.

6. **Hangfire Job Queue (Commented Out):**
   - The program includes commented-out code to demonstrate the use of Hangfire for background job processing.
   - Hangfire can be uncommented to enable background job enqueueing for further optimization and parallelism.

## Usage

1. The program generates or reads a CSV file named `Orders.csv` in the current directory.
2. Orders due today are processed asynchronously and parallelly for optimal performance.
3. The program outputs the number of orders processed for expiry and the total number of orders.

## Acknowledgments

- [CsvHelper](https://joshclose.github.io/CsvHelper/): A library for reading and writing CSV files in C#.