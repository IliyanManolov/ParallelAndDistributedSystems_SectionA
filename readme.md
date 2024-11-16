
## Program structure

The program includes 2 commented out scenarios for testing both tasks as well as commented out debug statements within the tasks themselves. 
In order to ease testing I have created 2 random generator classes - one for an array of `int` and one for a List of `Tool`.
Both tasks utilise local functions for auxiliary functions.
The `Tool` class for Task 2 has 2 properties - `Type` (int between 1 and 100) and `Barcode` (int - sequential coresponding to its location in the list, used to validate that no item tool has been selected twice)


Both tasks' execution times were tested with arrays of 100 000 elements as per the requirements listed in the Module book

## Task 1 Results

Thread count : Execution time in seconds using [s,fffffff] format

- 2 : 7,2684374
- 3 : 4,8035566
- 4 : 3,9155506
- 6 : 3,4843255

## Task 1 Evalution:

- Is this problem able to be parallelized? 
    - Yes
- How would the problem be partitioned?
    - Divide the array into "chunks" that do not overlap with each other and sort each chunk in parralel. Do an in-place merge at the end
- Are communications needed?
    - In this approach, no
- Are there any data dependencies?
    - In this approach, no. Normally the bubble sort algorithm depends on the current index and its neighbouring values, in the current approach the chunks do not interfere with each other
- Are there synchronization needs?
    - In this approach, no
- Will load balancing be a concern?
    - In this approach, yes. Some threads might end up with less sorted chunks causing them to perform more operations than others



## Task 2 Results

Thread count : Execution time in seconds using [s,fffffff] format

- 2 : 0,0024175
- 3 : 0,0024305
- 4 : 0,0023834
- 6 : 0,0025217


## Task 2 Evalution:

- Is this problem able to be parallelized? 
    - Yes
- How would the problem be partitioned?
    - Divide the collection of random tools into chunks that do not overlap and iterate through each item in a chunk in parallel
- Are communications needed?
    - Yes, in order to terminate the program as soon as all required tools are collected as well as to avoid taking too many tools of 1 particular type
- Are there any data dependencies?
    - No, the threads do not share what and how many tools each of them have found but do so as a collective
- Are there synchronization needs?
    - Yes, in order to not exceed the required number of tools of a particular type
- Will load balancing be a concern?
    - Yes, in case of a large number of threads some of them might spend time idling while waiting for the locks to be opened