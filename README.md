# RSSSorter
A little something I've thrown together to manage my gmail news alerts. I have enough different alerts nowadays that its getting tough to keep track of them all. Thankfully these alerts can be sent as an rss feed instead.
This program will take a folder of text files, each being a line separated list of rss feed urls, a list for highvalue and discardable alert urls, an output folder, and an optional age limit (defaults to 30 days).
For each list in the target folder, a pair of csv's with the same name are created and maintained in the output folder containing the specific rss feed to generate the alert line, the alert's title, the url to it, and timestamp for the last time the alert was updated.
Csv's are deduplicated and sorted by age, sufficiently old entries are removed.
